import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { delay, finalize } from "rxjs/operators";
import { BusyService } from "../services/busy.service";


@Injectable()
export class LoadingInterceptor implements HttpInterceptor{

    constructor(private busyService: BusyService){

    }
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
            this.busyService.busy();
            return next.handle(req).pipe(
          //COMMENT: Adding a delay before getting any HttpRequest to test Loading indicators.
                delay(1000),
                finalize (()=> {
                    this.busyService.idle();
                })
            );  
    }

}