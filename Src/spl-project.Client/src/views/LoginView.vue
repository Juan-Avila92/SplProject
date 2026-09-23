```vue
<script>
  import axios from 'axios'

  export default {
    name: 'LoginView',
    data() {
      return {
        email: '',
        password: '',
        showPassword: false
      }
    },

    mounted() {
      console.log('showPassword:', this.showPassword)
    },
    watch() {
      console.log('showPassword:', this.showPassword)
    },
    methods: {
      togglePassword: togglePassword,
      login: login
    }
  }

  function togglePassword() {
    this.showPassword = !this.showPassword
  }

  async function login() {
    try {
      const response = await axios.post('/api/auth/login', {
        email: this.email,
        password: this.password
      })

      console.log('Login successful:', response.data)

      localStorage.setItem('token', response.data.accessToken)

      this.$toast.success(
        'You have been successfully logged in.',
        'Login successful'
      )

      this.$router.push({
        name: 'DashboardView'
      })

    } catch (error) {
      this.$toast.error(
        error.response?.data?.message || 'Invalid email or password.',
        'Login failed'
      )
    }
  }


</script>

<template>
  <main class="app">
    <section class="">

      <!-- Login Card -->
      <div class="card">

        <p class="eyebrow">Welcome back</p>

        <h1>
          Keep moving<br />
          forward.
        </h1>

        <p class="intro">
          Sign in to pick up where you left off and stay close to what matters.
        </p>

        <form @submit.prevent="handleSubmit">

          <!-- Email -->
          <div class="field">
            <label for="email">
              Email address
            </label>

            <input id="email"
                   v-model="email"
                   name="email"
                   type="email"
                   autocomplete="email"
                   placeholder="you@example.com"
                   required
                   @input="clearStatus" />
          </div>

          <!-- Password -->
          <div class="field">

            <div class="password-row">
              <label for="password">
                Password
              </label>

              <button class="text-button"
                      type="button"
                      @click="">
                Forgot password?
              </button>
            </div>

            <div class="password-wrap">

              <!-- Lock icon -->
              <svg class="lock"
                   viewBox="0 0 24 24"
                   fill="none">
                <rect x="5"
                      y="10"
                      width="14"
                      height="10"
                      rx="2"
                      stroke="currentColor"
                      stroke-width="1.7" />

                <path d="M8 10V7a4 4 0 0 1 8 0v3"
                      stroke="currentColor"
                      stroke-width="1.7" />
              </svg>

              <input id="password"
                     v-model="password"
                     name="password"
                     :type="showPassword ? 'text' : 'password'"
                     autocomplete="current-password"
                     placeholder="Enter your password"
                     required
                     @input="clearStatus" />

              <!-- Show / Hide password -->
              <button class="icon-button"
                      type="button"
                      :aria-label="showPassword ? 'Hide password' : 'Show password'"
                      @click="togglePassword">
                <svg width="16"
                     height="16"
                     viewBox="0 0 24 24"
                     fill="none">
                  <path d="M2.5 12s3.5-6 9.5-6 9.5 6 9.5 6-3.5 6-9.5 6-9.5-6-9.5-6Z"
                        stroke="currentColor"
                        stroke-width="1.7" />

                  <circle cx="12"
                          cy="12"
                          r="2.5"
                          stroke="currentColor"
                          stroke-width="1.7" />
                </svg>
              </button>

            </div>
          </div>

          <!-- Submit -->
          <button class="submit" id="submit-button"
                  type="submit"
                  @click="login">
            <span id="submit-label">Sign in</span>
          </button>

        </form>
      </div>

      <!-- Create account -->
      <p class="signup">
        Don't have an account?

        <button type="button"
                @click="createAccount">
          Create an account
        </button>
      </p>

    </section>
  </main>
</template>

<style scoped>
  :global(*) {
    box-sizing: border-box;
  }

  :global(body) {
    margin: 0;
    min-height: 100vh;
    font-family: Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
  }

  .app {
    --ink: #13251e;
    --green: #173e2d;
    --green-hover: #255942;
    --muted: #718078;
    --line: #dbe5dc;
    --cream: #f5f7f2;
  }

  .page {
    position: relative;
    min-height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    overflow: hidden;
    padding: 32px 20px;
    color: var(--ink);
    background: cornflowerblue;
  }

    .page::before,
    .page::after {
      content: "";
      position: absolute;
      pointer-events: none;
      border-radius: 999px;
      filter: blur(48px);
    }

    .page::before {
      width: 256px;
      height: 256px;
      left: -112px;
      top: -112px;
      background: cornflowerblue;
    }

    .page::after {
      width: 288px;
      height: 288px;
      right: -96px;
      bottom: -128px;
      background: cornflowerblue;
    }

  .shell {
    position: relative;
    width: 100%;
    max-width: 420px;
  }

  .brand-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 24px;
    padding: 0 4px;
  }

  .brand {
    display: flex;
    align-items: center;
    gap: 10px;
    font-size: 15px;
    font-weight: 650;
    letter-spacing: -0.02em;
  }

  .brand-mark {
    width: 36px;
    height: 36px;
    display: grid;
    place-items: center;
    border-radius: 12px;
    color: #f4ddb0;
    background: cornflowerblue;
    box-shadow: 0 8px 20px rgba(23, 62, 45, 0.2);
  }

  .access {
    color: var(--muted);
    font-size: 11px;
    font-weight: 600;
    letter-spacing: 0.2em;
    text-transform: uppercase;
  }

  .card {
    padding: 36px;
    border: 1px solid #dfe7df;
    border-radius: 28px;
    background: rgba(255, 255, 255, 0.86);
    box-shadow: 0 24px 70px rgba(33, 62, 46, 0.1);
    backdrop-filter: blur(18px);
  }

  .eyebrow {
    margin: 0 0 12px;
    color: #6d887a;
    font-size: 11px;
    font-weight: 700;
    letter-spacing: 0.22em;
    text-transform: uppercase;
  }

  h1 {
    margin: 0;
    color: var(--green);
    font-size: 30px;
    line-height: 1.08;
    letter-spacing: -0.045em;
  }

  .intro {
    max-width: 280px;
    margin: 12px 0 32px;
    color: var(--muted);
    font-size: 13px;
    line-height: 1.55;
  }

  .field {
    margin-bottom: 20px;
  }

  label,
  .field-label {
    display: block;
    margin-bottom: 8px;
    color: cornflowerblue;
    font-size: 12px;
    font-weight: 650;
  }

  .password-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
  }

  input[type="email"],
  input[type="password"],
  input[type="text"] {
    width: 100%;
    height: 48px;
    border: 1px solid var(--line);
    border-radius: 12px;
    padding: 0 16px;
    color: var(--green);
    background: #fbfcfa;
    outline: none;
    font: inherit;
    font-size: 14px;
    transition: border-color 160ms ease, box-shadow 160ms ease;
  }

  input:focus {
    border-color: #5a9772;
    box-shadow: 0 0 0 4px rgba(90, 151, 114, 0.1);
  }

  input::placeholder {
    color: #a7b4aa;
  }

  .password-wrap {
    position: relative;
  }

    .password-wrap input {
      padding-left: 44px;
      padding-right: 48px;
    }

  .lock {
    position: absolute;
    left: 15px;
    top: 50%;
    width: 16px;
    height: 16px;
    transform: translateY(-50%);
    color: #9aac9f;
    pointer-events: none;
  }

  button {
    font: inherit;
    cursor: pointer;
  }

  .icon-button {
    position: absolute;
    right: 8px;
    top: 50%;
    width: 32px;
    height: 32px;
    display: grid;
    place-items: center;
    transform: translateY(-50%);
    border: 0;
    border-radius: 8px;
    color: #82958a;
    background: transparent;
  }

    .icon-button:hover {
      color: #294339;
      background: #edf4ee;
    }

  .text-button,
  .signup button {
    padding: 0;
    border: 0;
    color: #4f46e5;
    background: transparent;
    font-size: 11px;
    font-weight: 650;
  }

    .text-button:hover,
    .signup button:hover {
      color: darkslateblue;
    }

  .remember {
    display: flex;
    align-items: center;
    gap: 10px;
    margin: 2px 0 20px;
    color: var(--muted);
    font-size: 12px;
    cursor: pointer;
  }

    .remember input {
      position: absolute;
      opacity: 0;
      pointer-events: none;
    }

  .checkbox {
    width: 16px;
    height: 16px;
    display: grid;
    place-items: center;
    border: 1px solid #c9d8cc;
    border-radius: 5px;
    color: transparent;
    background: white;
  }

  .remember input:checked + .checkbox {
    border-color: #4d8b67;
    color: white;
    background: #4d8b67;
  }

  .status {
    margin-bottom: 16px;
    padding: 12px 14px;
    border: 1px solid #cbe7d1;
    border-radius: 12px;
    color: #32724b;
    background: #effaf1;
    font-size: 12px;
    font-weight: 550;
  }

  .submit {
    width: 100%;
    height: 48px;
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 8px;
    border: 0;
    border-radius: 12px;
    color: white;
    background: #4f46e5;
    box-shadow: 0 10px 24px rgba(23, 62, 45, 0.2);
    font-size: 13px;
    font-weight: 650;
    transition: background 160ms ease, transform 160ms ease, box-shadow 160ms ease;
  }

    .submit:hover {
      background: darkslateblue;
      box-shadow: 0 12px 28px rgba(23, 62, 45, 0.26);
    }

    .submit:active {
      transform: translateY(1px);
    }

  .divider {
    display: flex;
    align-items: center;
    gap: 12px;
    margin: 32px 0 16px;
    color: #a0ada3;
    font-size: 11px;
  }

    .divider::before,
    .divider::after {
      content: "";
      height: 1px;
      flex: 1;
      background: #e5ece5;
    }

  .google {
    width: 100%;
    height: 44px;
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 8px;
    border: 1px solid var(--line);
    border-radius: 12px;
    color: #294339;
    background: white;
    font-size: 12px;
    font-weight: 650;
  }

    .google:hover {
      border-color: #b9cfbd;
      background: #f8fbf8;
    }

  .google-mark {
    width: 20px;
    height: 20px;
    display: grid;
    place-items: center;
    border-radius: 999px;
    color: var(--green);
    background: #f2c94c;
    font-size: 10px;
    font-weight: 800;
  }

  .signup {
    margin: 24px 0 0;
    color: var(--muted);
    text-align: center;
    font-size: 12px;
  }

  @media (max-width: 480px) {
    .card {
      padding: 28px 24px;
    }

    .access {
      display: none;
    }

    h1 {
      font-size: 28px;
    }
  }
</style>
