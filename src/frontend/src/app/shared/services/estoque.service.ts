import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Produto } from '../models/produto.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EstoqueService {
  private apiUrl = environment.apiUrls.estoque + '/produtos';

  constructor(private http: HttpClient) { }

  criar(codigo: string, descricao: string, saldo: number): Observable<Produto> {
    return this.http.post<Produto>(this.apiUrl, { codigo, descricao, saldo });
  }

  obter(codigo: string): Observable<Produto> {
    return this.http.get<Produto>(`${this.apiUrl}/${codigo}`);
  }

  listar(): Observable<Produto[]> {
    return this.http.get<Produto[]>(this.apiUrl);
  }
}
