import { User } from './user';

export class UserParams {
  gender: string;
  orderBy = 'lastActive';
  minAge = 18;
  maxAge = 65;
  currentPageNumber = 1;
  itemsPerPage = 5;

  constructor(user: User | null) {
    this.gender = user?.gender === 'female' ? 'male' : 'female';
  }
}
