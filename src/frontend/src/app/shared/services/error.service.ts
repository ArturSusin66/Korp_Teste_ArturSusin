import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {
  private errorSubject = new Subject<string>();
  public error$ = this.errorSubject.asObservable();

  private successSubject = new Subject<string>();
  public success$ = this.successSubject.asObservable();

  mostrarErro(mensagem: string): void {
    this.errorSubject.next(mensagem);
  }

  mostrarSucesso(mensagem: string): void {
    this.successSubject.next(mensagem);
  }
}
