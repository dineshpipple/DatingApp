import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Dictionary } from '../_helper/dictionary';
import { Member } from '../_model/member';
import { PaginatedResult } from '../_model/pagination';
import { User } from '../_model/user';
import { UserParams } from '../_model/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  member: Member[] = [];
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();
  memberCache =  new Map();
  user:User;
  userParams:UserParams;


  constructor(private http: HttpClient,
    private accountService: AccountService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user=>{
        this.user = user;          
        this.userParams = new UserParams(user);
      })
     }

  getUserParams(){
    return this.userParams;
  }

  setUserParams(params: UserParams){
    this.userParams = params;
  }

  resetUserParams(){
    this.userParams= new UserParams(this.user);
    return this.userParams;
  } 

  getMembers(userParams: UserParams) {
    var response =  this.memberCache.get(Object.values(userParams).join('_'));    
    if(response){
      return of(response);
    }

    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params =  params.append('minAge',userParams.minAge.toString());
    params =  params.append('maxAge',userParams.maxAge.toString());
    params =  params.append('gender',userParams.gender);
    params =  params.append('orderBy',userParams.orderBy);
    
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'user',params)
      .pipe(map(response =>{        
        let paginatedResult = new PaginatedResult<Member[]>();        
        paginatedResult.pagination = response.pagination;
        paginatedResult.result = response.result;
        this.memberCache.set(Object.values(userParams).join("_"), paginatedResult);
        return response;
      }));
  }

  private getPaginatedResult<T>(url, params){
    const paginatedResult : PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<Member[]>(this.baseUrl + 'users', {observe: 'response', params}).pipe(
      map(response => {
        this.paginatedResult.result = response.body;
        if(response.headers.get('Pagination') != null){
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }        
        return this.paginatedResult;
      })
     )
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number){
    let params =  new HttpParams();
    params = params.append("pageNumber",pageNumber.toString());
    params = params.append("pageSize", pageSize.toString());

    return params;
  }

  getMember(username: string) {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member:Member) => member.username ==username);
    
    if(member){
      return of(member);
    }
    
    return this.http.get<Member>(this.baseUrl + 'users/' + username)
  }

  updateMember(member: Member){
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(()=>{
        const index = this.member.indexOf(member);
        this.member[index] = member;
      })
    );
  }

  setMainPhoto(photoId:number){
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {} )
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId)
  }
}
