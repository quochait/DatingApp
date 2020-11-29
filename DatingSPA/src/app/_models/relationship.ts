export interface Relationship{
  objectId: string;
  createdAt: Date;
  fromUserId: string;
  toUserId: string;
  status: string;
}