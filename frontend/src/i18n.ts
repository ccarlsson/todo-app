import { createI18n } from 'vue-i18n'

const messages = {
  sv: {
    app: {
      eyebrow: 'Klart & Tydligt',
      title: 'Klart & Tydligt',
      tagline: 'Håll koll på allt som behöver bli klart.',
    },
    actions: {
      close: 'Stäng',
      create: 'Skapa',
      save: 'Spara',
      update: 'Uppdatera',
      delete: 'Ta bort',
      back: 'Tillbaka',
      login: 'Logga in',
      register: 'Registrera',
      logout: 'Logga ut',
    },
    nav: {
      todos: 'Todos',
      newTodo: 'Ny todo',
      login: 'Logga in',
      register: 'Registrera',
    },
    auth: {
      loginTitle: 'Logga in',
      registerTitle: 'Registrera konto',
      email: 'E-post',
      password: 'Lösenord',
      noAccount: 'Inget konto?',
      haveAccount: 'Har du redan ett konto?',
      registerLink: 'Registrera dig',
    },
    validation: {
      emailRequired: 'E-post är obligatoriskt.',
      emailInvalid: 'Ange en giltig e-postadress.',
      passwordRequired: 'Lösenord är obligatoriskt.',
      passwordMin: 'Lösenord måste vara minst 6 tecken.',
      titleRequired: 'Titel är obligatoriskt.',
    },
    todos: {
      title: 'Dina todos',
      stats: 'Totalt: {total} · Öppna: {open} · Klara: {done}',
      empty: 'Inga todos ännu. Skapa din första!',
      statusLabel: 'Status',
      priorityLabel: 'Prioritet',
      dueLabel: 'Förfallo',
      status: {
        all: 'Alla status',
        notStarted: 'Ej påbörjad',
        inProgress: 'Pågående',
        completed: 'Klar',
      },
      priority: {
        all: 'Alla prioriteringar',
        low: 'Låg',
        medium: 'Medium',
        high: 'Hög',
      },
      dueFilter: {
        all: 'Alla förfallodatum',
        upcoming: 'Kommande',
        overdue: 'Förfallna',
      },
      sort: {
        createdAt: 'Sortera: Skapad',
        dueDate: 'Sortera: Förfallodatum',
      },
      createTitle: 'Skapa todo',
      editTitle: 'Redigera todo',
      formHint: 'Fyll i detaljerna nedan.',
      fields: {
        title: 'Titel',
        description: 'Beskrivning',
        dueDate: 'Förfallodatum',
        priority: 'Prioritet',
        status: 'Status',
      },
    },
  },
  en: {
    app: {
      eyebrow: 'Clear & Ready',
      title: 'Clear & Ready',
      tagline: 'Keep track of everything that needs to get done.',
    },
    actions: {
      close: 'Close',
      create: 'Create',
      save: 'Save',
      update: 'Refresh',
      delete: 'Delete',
      back: 'Back',
      login: 'Log in',
      register: 'Register',
      logout: 'Log out',
    },
    nav: {
      todos: 'Todos',
      newTodo: 'New todo',
      login: 'Log in',
      register: 'Register',
    },
    auth: {
      loginTitle: 'Log in',
      registerTitle: 'Create account',
      email: 'Email',
      password: 'Password',
      noAccount: "Don't have an account?",
      haveAccount: 'Already have an account? ',
      registerLink: 'Sign up',
    },
    validation: {
      emailRequired: 'Email is required.',
      emailInvalid: 'Enter a valid email address.',
      passwordRequired: 'Password is required.',
      passwordMin: 'Password must be at least 6 characters.',
      titleRequired: 'Title is required.',
    },
    todos: {
      title: 'Your todos',
      stats: 'Total: {total} · Open: {open} · Done: {done}',
      empty: 'No todos yet. Create your first!',
      statusLabel: 'Status',
      priorityLabel: 'Priority',
      dueLabel: 'Due',
      status: {
        all: 'All status',
        notStarted: 'Not started',
        inProgress: 'In progress',
        completed: 'Completed',
      },
      priority: {
        all: 'All priorities',
        low: 'Low',
        medium: 'Medium',
        high: 'High',
      },
      dueFilter: {
        all: 'All due dates',
        upcoming: 'Upcoming',
        overdue: 'Overdue',
      },
      sort: {
        createdAt: 'Sort: Created',
        dueDate: 'Sort: Due date',
      },
      createTitle: 'Create todo',
      editTitle: 'Edit todo',
      formHint: 'Fill in the details below.',
      fields: {
        title: 'Title',
        description: 'Description',
        dueDate: 'Due date',
        priority: 'Priority',
        status: 'Status',
      },
    },
  },
}

const STORAGE_KEY = 'todo_locale'
const savedLocale = localStorage.getItem(STORAGE_KEY) || 'sv'

export const i18n = createI18n({
  legacy: false,
  locale: savedLocale,
  fallbackLocale: 'sv',
  messages,
})

export function setLocale(locale: 'sv' | 'en') {
  i18n.global.locale.value = locale
  localStorage.setItem(STORAGE_KEY, locale)
}
