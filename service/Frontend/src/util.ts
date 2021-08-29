export const getLastMonday = (dt: Date) : Date => {
  // TODO check for daylight saving time changes
  const localDt = new Date(dt.getTime())
  localDt.setDate(localDt.getDate() - (localDt.getDay() + 6) % 7)
  return localDt
}
