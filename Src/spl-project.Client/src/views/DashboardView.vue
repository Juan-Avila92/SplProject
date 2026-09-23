```vue
<script>
  import axios from 'axios'

  export default {
    name: 'DashboardView',

    data() {
      return {
        dashboard: {
          userId: '',
          userName: '',
          role: '',
          totalProjects: 0,
          projects: []
        },
        loading: false,
        selectedTab: '',
        error: null,
        isProjectSelected: false,
        projectName: '',
        projectDescription: '',
        selectedProject: null,

        audioMeasuring: false,
        audioProgress: 0,
        soundLevel: null,
        microphoneError: null,
        audioContext: null,
        analyser: null,
        microphoneStream: null,
        currentMeasurementValue: null,
        measurementGroup: []
      }
    },

    mounted() {
      this.getDashboard()
    },

    methods: {
      async getDashboard() {
        this.setOverviewTab()
        this.loading = true
        this.error = null

        try {
          const token = localStorage.getItem('token')

          if (!token) {
            this.error = 'Authentication token is missing.'
            this.$router.push({ name: 'Login' })
            return
          }

          const response = await axios.get('/api/dashboard', {
            headers: {
              Authorization: `Bearer ${token}`
            }
          })

          this.dashboard = response.data

        }
        catch (error) {
          console.error('Error loading dashboard:', error)

          this.error =
            error.response?.data?.message ||
            'Unable to load dashboard.'
        }
        finally {
          this.loading = false
        }
      },
      async createProject() {
        this.name = ''
        try {
          const token = localStorage.getItem('token')

          if (!token) {
            console.error('Authentication token is missing.')
            this.$router.push({ name: 'Login' })
            return
          }

          const payload = {
            name: this.projectName,
            description: this.projectDescription
          }

          const response = await axios.post(
            '/api/project',
            payload,
            {
              headers: {
                Authorization: `Bearer ${token}`
              }
            }
          )

          const project = {
            id: response.data.id,
            name: response.data.name,
            description: response.data.description,
            createdAt: response.data.createdAt,
            updatedAt: response.data.updatedAt
          }

          this.dashboard.projects.push(project)

          this.$toast.success(
            'You have been successfullycreated a project.',
            'successful'
          )

          return response.data
        } catch (error) {
          console.error('Error creating project:', error)
        }

      },
      setProjectTab() {
        this.selectedTab = 'projects'
        this.isProjectSelected = false
      },
      setOverviewTab() {
        this.selectedTab = 'overview'
        this.isProjectSelected = false
      },
      openProjectModal() {
        const projectModal = document.querySelector("#project-modal");
        projectModal.hidden = false;
      },
      closeProjectModal() {
        const projectModal = document.querySelector("#project-modal");
        projectModal.hidden = true;
      },
      setSelectedproject(project) {
        this.selectedProject = project
        this.isProjectSelected = true
        this.selectedTab = ''
      },
      async measureSoundLevel() {
        this.soundMeasurements = []
        this.audioMeasuring = true

        try {
          const stream = await navigator.mediaDevices.getUserMedia({
            audio: {
              echoCancellation: false,
              noiseSuppression: false,
              autoGainControl: false
            }
          })

          this.microphoneStream = stream

          this.audioContext = new AudioContext()

          const source =
            this.audioContext.createMediaStreamSource(stream)

          this.analyser =
            this.audioContext.createAnalyser()

          this.analyser.fftSize = 2048

          source.connect(this.analyser)

          const buffer = new Float32Array(
            this.analyser.fftSize
          )
          let maxAmplitude = 0
          for (let i = 0; i < buffer.length; i++) {
            maxAmplitude = Math.max(
              maxAmplitude,
              Math.abs(buffer[i])
            )
          }

          console.log('Microphone amplitude:', maxAmplitude)

          for (let second = 1; second <= 5; second++) {

            const start = performance.now()

            let sumSquares = 0
            let sampleCount = 0

            while (performance.now() - start < 1000) {

              this.analyser.getFloatTimeDomainData(buffer)

              for (let i = 0; i < buffer.length; i++) {
                sumSquares += buffer[i] * buffer[i]
                sampleCount++
              }

              await new Promise(requestAnimationFrame)
            }

            const rms =
              Math.sqrt(sumSquares / sampleCount)

            const dbfs =
              20 * Math.log10(rms)

            this.currentMeasurementValue = dbfs.toFixed(2)

            this.soundMeasurements.push({
              second: second,
              level: dbfs.toFixed(2)
            })
          }

          this.measurementGroup.push(this.soundMeasurements)
          console.log(this.measurementGroup.length)
          console.log(this.measurementGroup)

        } catch (error) {

          console.error(
            'Microphone measurement failed:',
            error
          )

        } finally {

          this.stopAudioMeasurement()
        }
      },
      stopAudioMeasurement() {

        if (this.microphoneStream) {

          this.microphoneStream
            .getTracks()
            .forEach(track => track.stop())

          this.microphoneStream = null
        }

        if (this.audioContext) {

          this.audioContext.close()

          this.audioContext = null
        }

        this.audioMeasuring = false
      }
    }
  }

</script>

<template>
  <div class="app">
    <aside class="sidebar">
      <div class="brand">
        <span class="brand-mark">D</span>
        <span>Dashboard</span>
      </div>

      <p class="nav-label">Menu</p>
      <nav aria-label="Main navigation">
        <ul class="nav-list">
          <li>
            <a class="nav-link"
               href="#"
               :class="{ active: selectedTab === 'overview' }"
               @click.prevent="setOverviewTab">
              <span class="nav-icon" aria-hidden="true">⌂</span>
              Overview
            </a>
          </li>
          <li>
            <a class="nav-link"
               href="#"
               :class="{ active: selectedTab === 'projects' }"
               @click.prevent="setProjectTab">
              <span class="nav-icon" aria-hidden="true">▦</span>
              Projects
            </a>
          </li>
          <li>
            <a class="nav-link" href="#">
              <span class="nav-icon" aria-hidden="true">⚙</span>
              Settings
            </a>
          </li>
        </ul>
      </nav>
    </aside>

    <main class="main" id="overview">
      <header class="topbar">
        <div>
          <p class="eyebrow">Tuesday, September 22, 2026</p>
          <h1>Welcome back,</h1>
        </div>

        <!-- Replace the sample name and role with the signed-in user's data. -->
        <div class="user-card" aria-label="User profile">
          <div class="avatar" aria-hidden="true">AJ</div>
          <div>
            <p class="user-name">
              {{ dashboard.userName }}
            </p>
            <p class="user-role"> {{ dashboard.role }} </p>
          </div>
        </div>
      </header>
      <section aria-labelledby="summary-heading" v-if="selectedTab === 'overview'">
        <div class="section-heading">
          <h2 id="summary-heading">Overview</h2>
        </div>
        <div class="stats">
          <article class="stat-card">
            <p class="stat-label">Active projects</p>
            <p class="stat-value">{{ dashboard.totalProjects }} </p>
          </article>
          <article class="stat-card">
            <p class="stat-label">Tasks completed</p>
            <p class="stat-value">84</p>
          </article>
          <article class="stat-card">
            <p class="stat-label">Team members</p>
            <p class="stat-value">8</p>
          </article>
        </div>
      </section>

      <section aria-labelledby="summary-heading" v-if="isProjectSelected === true">
        <div class="section-heading">
          <h2 id="summary-heading">{{ selectedProject.name }}</h2>

        </div>
        <p>{{ selectedProject.description }}</p>
        <div class="stats">
          <button class="primary-button"
                  type="button"
                  @click="measureSoundLevel"
                  :disabled="audioMeasuring">
            {{ audioMeasuring ? 'Measuring...' : 'Start measurement' }}
          </button>
        </div>
        <section class="dbfs-card" aria-label="Audio level">
          <div class="dbfs-header">
            <p class="dbfs-label">Audio level</p>
            <p class="dbfs-value">{{ currentMeasurementValue }} dBFS</p>
          </div>

          <div class="meter" aria-hidden="true">
            <div class="meter-fill"></div>
            <div class="meter-marker"></div>
          </div>

          <div class="scale" aria-hidden="true">
            <span>-60</span>
            <span>-30</span>
            <span>-12</span>
            <span>0 dBFS</span>
          </div>

          <p class="dbfs-status">
            <span class="status-dot" aria-hidden="true"></span>
            Healthy signal
          </p>
        </section>
        <div class="measurements" v-if="measurementGroup.length > 0">
          <div v-for="measurement in measurementGroup"
               :key="measurement.second"
               class="measurement-card"
               style="background-color: cornflowerblue">

            <div class="measurement-level">
              {{  measurement.map(measurement => measurement.level).join(', ') }} dB
            </div>
          </div>
        </div>
      </section>

      <section id="projects" aria-labelledby="projects-heading" v-if="selectedTab === 'projects'">
        <div class="section-heading">
          <h2 id="projects-heading">Projects</h2>

          <button class="primary-button" id="add-project" type="button" @click="openProjectModal">
            + Add project
          </button>
        </div>
        <div class="projects">
          <article v-for="project in dashboard.projects"
                   :key="project.id"
                   class="project-card">
            <div class="project-icon" aria-hidden="true">✦</div>

            <h3>{{ project.name }}</h3>

            <p>{{ project.description }}</p>

            <a class="project-link"
               @click="setSelectedproject(project)">
              Open project →
            </a>
          </article>
        </div>
      </section>
    </main>
  </div>

  <div class="modal-backdrop"
       id="project-modal"
       role="presentation"
       hidden>
    <div class="modal"
         role="dialog"
         aria-modal="true"
         aria-labelledby="modal-title">
      <div class="modal-header">
        <div>
          <h2 id="modal-title">Add project</h2>
          <p class="modal-subtitle">Enter the details for your new project.</p>
        </div>
        <button class="close-button"
                id="close-modal"
                type="button"
                aria-label="Close modal"
                @click="closeProjectModal">
          ×
        </button>
      </div>
      <form id="project-form" @submit.prevent="createProject">
        <div class="form-group">
          <label for="project-name">Name</label>
          <input id="project-name"
                 v-model="projectName"
                 name="name"
                 type="text"
                 placeholder="e.g. Website redesign"
                 required />
        </div>
        <div class="form-group">
          <label for="project-description">Description</label>
          <textarea id="project-description"
                    name="description"
                    v-model="projectDescription"
                    rows="4"
                    placeholder="What is this project about?"
                    required></textarea>
        </div>
        <div class="modal-actions">
          <button class="action-button" id="cancel-modal" type="button">
            Cancel
          </button>
          <button class="primary-button" type="submit">
            Add project
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
<style scoped>
  .app {
    --background: #f6f8fb;
    --surface: #ffffff;
    --text: #172033;
    --muted: #6b7280;
    --border: #e5e7eb;
    --primary: #4f46e5;
    --primary-light: #eef2ff;
    --sidebar: #111827;
    --sidebar-muted: #9ca3af;
    display: flex;
    min-height: 100vh;
  }

  * {
    box-sizing: border-box;
  }

  .app {
    display: flex;
    min-height: 100vh;
  }

  .measurements {
    display: flex;
    gap: 16px;
    flex-wrap: wrap;
    margin-top: 20px;
  }

  .measurement-card {
    width: 160px;
    padding: 20px;
    border-radius: 12px;
    background: #ffffff;
    border: 1px solid #e5e7eb;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  }

  .measurement-header {
    margin-bottom: 12px;
  }

  .measurement-second {
    font-size: 14px;
    font-weight: 600;
    color: #6b7280;
  }

  .measurement-level {
    font-size: 28px;
    font-weight: 700;
  }

  body {
    display: grid;
    min-height: 100vh;
    margin: 0;
    padding: 24px;
    place-items: center;
    background: #f6f8fb;
    color: #172033;
    font-family: Inter, ui-sans-serif, system-ui, sans-serif;
  }

  .dbfs-card {
    width: min(100%, 330px);
    padding: 20px;
    border: 1px solid #e5e7eb;
    border-radius: 14px;
    background: #ffffff;
    box-shadow: 0 8px 24px rgb(17 24 39 / 6%);
  }

  .dbfs-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 18px;
  }

  .dbfs-label {
    margin: 0;
    color: #6b7280;
    font-size: 0.78rem;
    font-weight: 700;
    letter-spacing: 0.08em;
    text-transform: uppercase;
  }

  .dbfs-value {
    margin: 0;
    font-size: 1.45rem;
    font-weight: 700;
    letter-spacing: -0.04em;
  }

  .meter {
    position: relative;
    height: 12px;
    overflow: hidden;
    border-radius: 999px;
    background: linear-gradient( to right, #22c55e 0%, #22c55e 55%, #facc15 72%, #f97316 86%, #ef4444 100% );
  }

  .meter-fill {
    position: absolute;
    inset: 0 34% 0 0;
    border-radius: inherit;
    background: rgb(255 255 255 / 72%);
  }

  .meter-marker {
    position: absolute;
    top: -4px;
    left: 66%;
    width: 3px;
    height: 20px;
    border-radius: 4px;
    background: #172033;
    box-shadow: 0 0 0 2px white;
  }

  .scale {
    display: flex;
    justify-content: space-between;
    margin-top: 8px;
    color: var(--muted);
    font-size: 0.7rem;
  }

  .dbfs-status {
    display: flex;
    align-items: center;
    gap: 7px;
    margin: 16px 0 0;
    color: #15803d;
    font-size: 0.78rem;
    font-weight: 600;
  }

  .status-dot {
    width: 7px;
    height: 7px;
    border-radius: 50%;
    background: #22c55e;
  }

  .primary-button {
    padding: 9px 14px;
    border: none;
    border-radius: 8px;
    background: #4f46e5;
    color: #ffffff;
    cursor: pointer;
    font-family: inherit;
    font-size: 0.82rem;
    font-weight: 700;
    transition: background 160ms ease, transform 160ms ease;
  }

    .primary-button:hover {
      background: #4338ca;
      transform: translateY(-1px);
    }

  .sidebar {
    width: 250px;
    padding: 28px 18px;
    background: var(--sidebar);
    color: white;
  }

  .brand {
    display: flex;
    align-items: center;
    gap: 10px;
    margin: 0 12px 42px;
    font-size: 1.15rem;
    font-weight: 700;
  }

  .brand-mark {
    display: grid;
    width: 32px;
    height: 32px;
    place-items: center;
    border-radius: 9px;
    background: var(--primary);
    font-size: 0.9rem;
  }

  .nav-label {
    margin: 0 12px 10px;
    color: var(--sidebar-muted);
    font-size: 0.72rem;
    font-weight: 700;
    letter-spacing: 0.08em;
    text-transform: uppercase;
  }

  .nav-list {
    display: grid;
    gap: 6px;
    margin: 0;
    padding: 0;
    list-style: none;
  }

  .nav-link {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 11px 12px;
    border-radius: 8px;
    color: var(--sidebar-muted);
    font-size: 0.94rem;
    text-decoration: none;
    transition: background 160ms ease, color 160ms ease;
  }

    .nav-link:hover,
    .nav-link.active {
      background: #1f2937;
      color: white;
    }

    .nav-link.active {
      box-shadow: inset 3px 0 0 var(--primary);
    }

  .nav-icon {
    width: 20px;
    text-align: center;
    font-size: 1rem;
  }

  .main {
    width: 100%;
    max-width: 1300px;
    margin: 0 auto;
    padding: 34px 42px;
  }

  .topbar {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    gap: 24px;
    margin-bottom: 34px;
  }

  .eyebrow {
    margin: 0 0 8px;
    color: var(--muted);
    font-size: 0.86rem;
  }

  h1 {
    margin: 0;
    font-size: clamp(1.7rem, 3vw, 2.25rem);
    letter-spacing: -0.04em;
  }

  .user-card {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 8px 12px 8px 8px;
    border: 1px solid var(--border);
    border-radius: 12px;
    background: var(--surface);
  }

  .avatar {
    display: grid;
    width: 40px;
    height: 40px;
    place-items: center;
    border-radius: 50%;
    background: var(--primary-light);
    color: var(--primary);
    font-size: 0.9rem;
    font-weight: 700;
  }

  .user-name,
  .user-role {
    margin: 0;
  }

  .user-name {
    font-size: 0.9rem;
    font-weight: 700;
  }

  .user-role {
    margin-top: 3px;
    color: var(--muted);
    font-size: 0.78rem;
  }

  .section-heading {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 16px;
  }

  h2 {
    margin: 0;
    font-size: 1.15rem;
  }

  .section-heading a {
    color: var(--primary);
    font-size: 0.86rem;
    font-weight: 600;
    text-decoration: none;
  }

    .section-heading a:hover {
      text-decoration: underline;
    }

  .stats {
    display: grid;
    grid-template-columns: repeat(3, minmax(0, 1fr));
    gap: 18px;
    margin-bottom: 38px;
  }

  .stat-card,
  .project-card {
    border: 1px solid var(--border);
    border-radius: 14px;
    background: var(--surface);
  }

  .stat-card {
    padding: 20px;
  }

  .stat-label {
    margin: 0 0 10px;
    color: var(--muted);
    font-size: 0.84rem;
  }

  .stat-value {
    margin: 0;
    font-size: 1.8rem;
    letter-spacing: -0.04em;
  }

  .projects {
    display: grid;
    grid-template-columns: repeat(3, minmax(0, 1fr));
    gap: 18px;
  }

  .project-card {
    padding: 20px;
    transition: border-color 160ms ease, box-shadow 160ms ease, transform 160ms ease;
  }

    .project-card:hover {
      border-color: #c7d2fe;
      box-shadow: 0 8px 24px rgb(79 70 229 / 8%);
      transform: translateY(-2px);
    }

  .project-icon {
    display: grid;
    width: 42px;
    height: 42px;
    margin-bottom: 18px;
    place-items: center;
    border-radius: 10px;
    background: var(--primary-light);
    color: var(--primary);
    font-size: 1.1rem;
  }

  .project-card h3 {
    margin: 0 0 7px;
    font-size: 1rem;
  }

  .project-card p {
    min-height: 42px;
    margin: 0 0 18px;
    color: var(--muted);
    font-size: 0.86rem;
    line-height: 1.55;
  }

  .project-link {
    color: var(--primary);
    font-size: 0.86rem;
    font-weight: 700;
    text-decoration: none;
  }

    .project-link:hover {
      text-decoration: underline;
    }


  .modal-backdrop {
    position: fixed;
    z-index: 10;
    inset: 0;
    display: grid;
    padding: 20px;
    place-items: center;
    background: rgb(17 24 39 / 50%);
  }

    .modal-backdrop[hidden] {
      display: none;
    }

  .modal {
    width: min(100%, 460px);
    padding: 24px;
    border-radius: 14px;
    background: #ffffff;
    box-shadow: 0 20px 50px rgb(17 24 39 / 20%);
  }

  .modal-header {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    gap: 16px;
    margin-bottom: 22px;
  }

  .modal h2 {
    margin: 0 0 6px;
    color: #172033;
    font-size: 1.2rem;
  }

  .modal-subtitle {
    margin: 0;
    color: #6b7280;
    font-size: 0.84rem;
  }

  .close-button {
    width: 30px;
    height: 30px;
    border: none;
    border-radius: 7px;
    background: transparent;
    color: #6b7280;
    cursor: pointer;
    font-size: 1.25rem;
    line-height: 1;
  }

    .close-button:hover {
      background: #f6f8fb;
      color: #172033;
    }

  .form-group {
    display: grid;
    gap: 7px;
    margin-bottom: 17px;
  }

    .form-group label {
      color: #172033;
      font-size: 0.84rem;
      font-weight: 700;
    }

    .form-group input,
    .form-group textarea {
      width: 100%;
      padding: 10px 12px;
      border: 1px solid #e5e7eb;
      border-radius: 8px;
      color: #172033;
      font: inherit;
      font-size: 0.88rem;
      outline: none;
      resize: vertical;
    }

      .form-group input:focus,
      .form-group textarea:focus {
        border-color: #4f46e5;
        box-shadow: 0 0 0 3px #eef2ff;
      }

  .modal-actions {
    display: flex;
    justify-content: flex-end;
    gap: 9px;
    margin-top: 24px;
  }

  @media (max-width: 820px) {
    .sidebar {
      width: 205px;
    }

    .main {
      padding: 28px 24px;
    }

    .stats,
    .projects {
      grid-template-columns: repeat(2, minmax(0, 1fr));
    }
  }

  @media (max-width: 620px) {
    .app {
      display: block;
    }

    .sidebar {
      width: 100%;
      padding: 18px;
    }

    .brand {
      margin-bottom: 20px;
    }

    .nav-list {
      grid-template-columns: repeat(3, 1fr);
    }

    .nav-label {
      display: none;
    }

    .nav-link {
      justify-content: center;
      padding: 10px 6px;
    }

    .main {
      padding: 26px 18px;
    }

    .topbar {
      display: block;
    }

    .user-card {
      width: fit-content;
      margin-top: 18px;
    }

    .stats,
    .projects {
      grid-template-columns: 1fr;
    }
  }
</style>
