import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from 'src/app/account/account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private accountSerivce: AccountService, private router: Router){

  }


  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> {
    return this.accountSerivce.currentUser$.pipe(
      map(auth => {
         if (auth) {
           console.log("Auth.")
          return true;
         }
         console.log("Auth.2")
         this.router.navigate(['account/login'], {queryParams: {returnUrl: state.url}});
      })
    );
  }
  
}
