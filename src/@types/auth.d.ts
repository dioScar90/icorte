import { User } from 'firebase/auth'

export type AuthContextType = {
  user: User | null
  isLoading?: boolean
  register?: (email: string, password: string) => Promise<UserCredential>
  login?: (email: string, password: string) => Promise<UserCredential>
  loginWithGoogle?: () => Promise<UserCredential>
  logout?: () => Promise<UserCredential>
  resetPassword?: (email: string) => Promise<void>
  changePassword?: (password: string) => Promise<void>
}
