export const getAccountInfo = async () => await fetch('/api/account/info').then(val => val.json())
export const register = async (employeeName: string, department: string) => {
  const form = new FormData()
  form.append('employeeName', employeeName)
  form.append('department', department)
  await fetch('/api/account/register', {
    method: 'POST',
    body: form
  })
}
