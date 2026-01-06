window.getLocale = () => navigator.language;
window.getTimezone = () => Intl.DateTimeFormat().resolvedOptions().timeZone;