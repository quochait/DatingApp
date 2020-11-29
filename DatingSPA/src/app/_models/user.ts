import { Photo } from './Photo';

export interface User {
  objectId: string;
  username: string;
  knownAs: string;
  age: number;
  gender: string;
  createdAt: Date;
  lastActive: Date;
  photoUrl: string;
  city: string;
  country: string;
  interests?: string;
  introduction?: string;
  lookingFor?: string;
  isVerifyEmail?: boolean;
  email?: string;
  mainPhotoUrl?: string;
}
