export interface DirectoryUser {
  id: string;
  displayName: string;
  userName: string | null;
  email: string | null;
  pictureUrl: string | null;
  lastSeenAtUtc: string | null;
}
