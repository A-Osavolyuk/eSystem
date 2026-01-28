export const AuthenticationEvents = {
  LOGOUT: "logout",
  LOGIN: "login",
} as const;

export type AuthenticationEvent = typeof AuthenticationEvents[keyof typeof AuthenticationEvents];
