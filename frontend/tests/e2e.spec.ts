import { test, expect } from '@playwright/test'

function uniqueEmail() {
  const stamp = Date.now()
  return `e2e_${stamp}@example.com`
}

test('redirects unauthenticated users to login', async ({ page }) => {
  await page.goto('/todos')
  await expect(page.getByRole('heading', { name: 'Logga in' })).toBeVisible()
})

test('register, create todo, and delete todo', async ({ page }) => {
  const email = uniqueEmail()
  const password = 'Password123!'

  await page.goto('/register')
  await page.getByLabel('E‑post').fill(email)
  await page.getByLabel('Lösenord').fill(password)
  await page.getByRole('button', { name: 'Skapa konto' }).click()

  await expect(page.getByRole('heading', { name: 'Dina todos' })).toBeVisible()

  await page.getByRole('link', { name: 'Ny todo' }).click()
  await expect(page.getByRole('heading', { name: 'Skapa todo' })).toBeVisible()

  const title = `E2E todo ${Date.now()}`
  await page.getByLabel('Titel').fill(title)
  await page.getByRole('button', { name: 'Skapa' }).click()

  await expect(page.getByRole('heading', { name: 'Dina todos' })).toBeVisible()

  const todoItem = page.locator('.todo-item', { hasText: title })
  await expect(todoItem).toBeVisible()

  await todoItem.getByRole('button', { name: 'Ta bort' }).click()
  await expect(todoItem).toHaveCount(0)
})
