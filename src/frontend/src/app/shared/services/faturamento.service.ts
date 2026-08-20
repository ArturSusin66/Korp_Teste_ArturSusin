import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NotaFiscal } from '../models/nota-fiscal.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FaturamentoService {
  private apiUrl = environment.apiUrls.faturamento + '/notas-fiscais';

  constructor(private http: HttpClient) { }

  criar(): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(this.apiUrl, {});
  }

  obter(numero: number): Observable<NotaFiscal> {
    return this.http.get<NotaFiscal>(`${this.apiUrl}/${numero}`);
  }

  listar(): Observable<NotaFiscal[]> {
    return this.http.get<NotaFiscal[]>(this.apiUrl);
  }

  adicionarItem(numero: number, codigoProduto: string, quantidade: number, valor: number): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(
      `${this.apiUrl}/${numero}/itens`,
      { codigoProduto, quantidade, valor }
    );
  }

  imprimir(numero: number): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(`${this.apiUrl}/${numero}/imprimir`, {});
  }
}
