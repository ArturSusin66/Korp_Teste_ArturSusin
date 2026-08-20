import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FaturamentoService } from '../../shared/services/faturamento.service';
import { EstoqueService } from '../../shared/services/estoque.service';
import { ErrorService } from '../../shared/services/error.service';
import { NotaFiscal, ItemNotaFiscal } from '../../shared/models/nota-fiscal.model';
import { Produto } from '../../shared/models/produto.model';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-notas-fiscais',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="container">
      <h2>📄 Notas Fiscais</h2>

      <!-- Mensagens -->
      <div *ngIf="(errorService.error$ | async) as erro" class="alert alert-error">
        ❌ {{ erro }}
      </div>
      <div *ngIf="(errorService.success$ | async) as sucesso" class="alert alert-success">
        ✅ {{ sucesso }}
      </div>

      <!-- Criador de Nota -->
      <div *ngIf="!notaAtual" class="form-section">
        <h3>Criar Nova Nota Fiscal</h3>
        <button (click)="criarNota()" [disabled]="carregandoNova">
          {{ carregandoNova ? 'Criando...' : '➕ Nova Nota Fiscal' }}
        </button>
      </div>

      <!-- Editor de Nota -->
      <div *ngIf="notaAtual" class="form-section nota-editor">
        <div class="nota-header">
          <h3>Nota Fiscal NF-{{ notaAtual.numero | string }}</h3>
          <span class="status" [ngClass]="'status-' + notaAtual.status.toLowerCase()">
            {{ notaAtual.status }}
          </span>
        </div>

        <!-- Adicionar Item -->
        <div *ngIf="notaAtual.status === 'Aberta'" class="item-form">
          <h4>Adicionar Item</h4>
          <form [formGroup]="itemForm" (ngSubmit)="adicionarItem()">
            <div class="form-row">
              <div class="form-group">
                <label for="codigoProduto">Produto:</label>
                <select id="codigoProduto" formControlName="codigoProduto">
                  <option value="">-- Selecione um produto --</option>
                  <option *ngFor="let p of produtos" [value]="p.codigo">
                    {{ p.codigo }} - {{ p.descricao }} (Saldo: {{ p.saldo }})
                  </option>
                </select>
              </div>

              <div class="form-group">
                <label for="quantidade">Quantidade:</label>
                <input
                  id="quantidade"
                  type="number"
                  formControlName="quantidade"
                  min="1"
                />
              </div>

              <div class="form-group">
                <label for="valor">Valor Unitário:</label>
                <input
                  id="valor"
                  type="number"
                  formControlName="valor"
                  min="0.01"
                  step="0.01"
                />
              </div>

              <button type="submit" [disabled]="itemForm.invalid || carregandoItem">
                {{ carregandoItem ? 'Adicionando...' : 'Adicionar' }}
              </button>
            </div>
          </form>
        </div>

        <!-- Itens Adicionados -->
        <div class="itens-section" *ngIf="notaAtual.itens.length > 0">
          <h4>Itens da Nota</h4>
          <table class="table">
            <thead>
              <tr>
                <th>Produto</th>
                <th>Quantidade</th>
                <th>Valor Unit.</th>
                <th>Subtotal</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let item of notaAtual.itens">
                <td>{{ item.codigoProduto }}</td>
                <td>{{ item.quantidade }}</td>
                <td>{{ item.valor | currency }}</td>
                <td class="subtotal">{{ (item.valor * item.quantidade) | currency }}</td>
              </tr>
            </tbody>
            <tfoot>
              <tr class="total-row">
                <td colspan="3">TOTAL:</td>
                <td class="total-value">{{ notaAtual.total | currency }}</td>
              </tr>
            </tfoot>
          </table>
        </div>

        <!-- Ações -->
        <div class="actions">
          <button (click)="voltarLista()" class="btn-secondary">
            ← Voltar
          </button>
          <button
            *ngIf="notaAtual.status === 'Aberta' && notaAtual.itens.length > 0"
            (click)="imprimirNota()"
            [disabled]="carregandoImpressao"
            class="btn-primary"
          >
            <span *ngIf="carregandoImpressao" class="spinner"></span>
            {{ carregandoImpressao ? 'Processando...' : '🖨️ Imprimir Nota' }}
          </button>
        </div>
      </div>

      <!-- Lista de Notas -->
      <div *ngIf="!notaAtual" class="list-section">
        <h3>Notas Fiscais Cadastradas</h3>
        <div *ngIf="carregandoLista" class="loading">Carregando notas...</div>
        <table *ngIf="!carregandoLista && notas.length > 0" class="table">
          <thead>
            <tr>
              <th>NF</th>
              <th>Status</th>
              <th>Itens</th>
              <th>Total</th>
              <th>Data Emissão</th>
              <th>Ação</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let nota of notas">
              <td><strong>{{ nota.numero }}</strong></td>
              <td>
                <span class="status" [ngClass]="'status-' + nota.status.toLowerCase()">
                  {{ nota.status }}
                </span>
              </td>
              <td>{{ nota.itens.length }}</td>
              <td>{{ nota.total | currency }}</td>
              <td>{{ nota.dataEmissao | date: 'dd/MM/yyyy HH:mm' }}</td>
              <td>
                <button (click)="editarNota(nota.numero)" class="btn-edit">
                  Visualizar
                </button>
              </td>
            </tr>
          </tbody>
        </table>
        <div *ngIf="!carregandoLista && notas.length === 0" class="no-data">
          Nenhuma nota fiscal cadastrada
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container {
      max-width: 1000px;
      margin: 0 auto;
    }

    h2 {
      color: #2c3e50;
      margin-bottom: 20px;
    }

    h3, h4 {
      color: #34495e;
      margin: 15px 0;
    }

    .alert {
      padding: 15px;
      margin-bottom: 20px;
      border-radius: 4px;
      animation: slideIn 0.3s ease-in;
    }

    .alert-success {
      background-color: #d4edda;
      color: #155724;
      border: 1px solid #c3e6cb;
    }

    .alert-error {
      background-color: #f8d7da;
      color: #721c24;
      border: 1px solid #f5c6cb;
    }

    @keyframes slideIn {
      from { opacity: 0; transform: translateY(-10px); }
      to { opacity: 1; transform: translateY(0); }
    }

    .form-section, .list-section {
      background: white;
      padding: 20px;
      border-radius: 4px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      margin-bottom: 20px;
    }

    .nota-editor {
      border-left: 5px solid #3498db;
    }

    .nota-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
      padding-bottom: 10px;
      border-bottom: 2px solid #ecf0f1;
    }

    .status {
      padding: 5px 10px;
      border-radius: 20px;
      font-size: 12px;
      font-weight: 600;
    }

    .status-aberta {
      background-color: #d4edda;
      color: #155724;
    }

    .status-fechada {
      background-color: #e2e3e5;
      color: #383d41;
    }

    .item-form {
      background-color: #f8f9fa;
      padding: 15px;
      border-radius: 4px;
      margin-bottom: 20px;
    }

    .form-row {
      display: grid;
      grid-template-columns: 2fr 1fr 1fr auto;
      gap: 10px;
      align-items: flex-end;
    }

    .form-group {
      display: flex;
      flex-direction: column;
    }

    label {
      margin-bottom: 5px;
      font-weight: 600;
      color: #34495e;
      font-size: 13px;
    }

    input, select {
      padding: 8px;
      border: 1px solid #bdc3c7;
      border-radius: 4px;
      font-size: 14px;
    }

    input:focus, select:focus {
      outline: none;
      border-color: #3498db;
      box-shadow: 0 0 5px rgba(52, 152, 219, 0.3);
    }

    button {
      padding: 10px 20px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 14px;
      font-weight: 600;
      transition: all 0.3s ease;
    }

    button:not([disabled]):hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 8px rgba(0,0,0,0.15);
    }

    button:disabled {
      background-color: #95a5a6;
      cursor: not-allowed;
      opacity: 0.6;
    }

    .form-section > button:not([disabled]) {
      background-color: #27ae60;
      color: white;
    }

    .item-form button {
      background-color: #3498db;
      color: white;
    }

    .btn-primary {
      background-color: #27ae60;
      color: white;
    }

    .btn-secondary {
      background-color: #95a5a6;
      color: white;
    }

    .btn-edit {
      background-color: #3498db;
      color: white;
      padding: 6px 12px;
      font-size: 12px;
    }

    .table {
      width: 100%;
      border-collapse: collapse;
      font-size: 14px;
      margin: 15px 0;
    }

    .table thead {
      background-color: #ecf0f1;
      font-weight: 600;
      color: #2c3e50;
    }

    .table th, .table td {
      padding: 12px;
      text-align: right;
      border-bottom: 1px solid #bdc3c7;
    }

    .table th:first-child, .table td:first-child {
      text-align: left;
    }

    .table tbody tr:hover {
      background-color: #f9f9f9;
    }

    .table tfoot {
      font-weight: 600;
      background-color: #ecf0f1;
    }

    .total-row td {
      padding: 15px 12px;
    }

    .total-value {
      color: #27ae60;
      font-size: 16px;
    }

    .itens-section {
      margin: 20px 0;
    }

    .actions {
      display: flex;
      gap: 10px;
      margin-top: 20px;
      justify-content: space-between;
    }

    .spinner {
      display: inline-block;
      width: 14px;
      height: 14px;
      border: 2px solid rgba(255,255,255,0.3);
      border-radius: 50%;
      border-top-color: white;
      animation: spin 0.6s linear infinite;
      margin-right: 8px;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    .loading, .no-data {
      text-align: center;
      padding: 40px 20px;
      color: #7f8c8d;
      font-size: 16px;
    }
  `]
})
export class NotasFiscaisComponent implements OnInit, OnDestroy {
  notas: NotaFiscal[] = [];
  notaAtual: NotaFiscal | null = null;
  produtos: Produto[] = [];
  itemForm: FormGroup;
  carregando = false;
  carregandoLista = false;
  carregandoNova = false;
  carregandoItem = false;
  carregandoImpressao = false;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private faturamentoService: FaturamentoService,
    private estoqueService: EstoqueService,
    public errorService: ErrorService
  ) {
    this.itemForm = this.fb.group({
      codigoProduto: ['', Validators.required],
      quantidade: [1, [Validators.required, Validators.min(1)]],
      valor: [0, [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    this.carregarNotas();
    this.carregarProdutos();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  criarNota(): void {
    this.carregandoNova = true;
    this.faturamentoService.criar()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (nota) => {
          this.notaAtual = nota;
          this.errorService.mostrarSucesso(`Nota Fiscal NF-${nota.numero} criada!`);
          this.carregandoNova = false;
        },
        error: (error) => {
          const mensagem = error.error?.mensagem || 'Erro ao criar nota';
          this.errorService.mostrarErro(mensagem);
          this.carregandoNova = false;
        }
      });
  }

  editarNota(numero: number): void {
    this.carregando = true;
    this.faturamentoService.obter(numero)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (nota) => {
          this.notaAtual = nota;
          this.carregando = false;
        },
        error: (error) => {
          const mensagem = error.error?.mensagem || 'Erro ao carregar nota';
          this.errorService.mostrarErro(mensagem);
          this.carregando = false;
        }
      });
  }

  voltarLista(): void {
    this.notaAtual = null;
    this.itemForm.reset();
    this.carregarNotas();
  }

  adicionarItem(): void {
    if (this.itemForm.invalid || !this.notaAtual) return;

    const { codigoProduto, quantidade, valor } = this.itemForm.value;

    this.carregandoItem = true;
    this.faturamentoService.adicionarItem(this.notaAtual.numero, codigoProduto, quantidade, valor)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (nota) => {
          this.notaAtual = nota;
          this.itemForm.reset();
          this.errorService.mostrarSucesso('Item adicionado com sucesso!');
          this.carregandoItem = false;
        },
        error: (error) => {
          const mensagem = error.error?.mensagem || 'Erro ao adicionar item';
          this.errorService.mostrarErro(mensagem);
          this.carregandoItem = false;
        }
      });
  }

  imprimirNota(): void {
    if (!this.notaAtual) return;

    this.carregandoImpressao = true;
    this.faturamentoService.imprimir(this.notaAtual.numero)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (nota) => {
          this.notaAtual = nota;
          this.errorService.mostrarSucesso('Nota fiscal impressa com sucesso! Estoque atualizado.');
          this.carregandoImpressao = false;
          setTimeout(() => this.voltarLista(), 2000);
        },
        error: (error) => {
          const mensagem = error.error?.mensagem || 'Erro ao imprimir nota';
          this.errorService.mostrarErro(mensagem);
          this.carregandoImpressao = false;
        }
      });
  }

  private carregarNotas(): void {
    this.carregandoLista = true;
    this.faturamentoService.listar()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (notas) => {
          this.notas = notas.sort((a, b) => b.numero - a.numero);
          this.carregandoLista = false;
        },
        error: (error) => {
          const mensagem = error.error?.mensagem || 'Erro ao carregar notas';
          this.errorService.mostrarErro(mensagem);
          this.carregandoLista = false;
        }
      });
  }

  private carregarProdutos(): void {
    this.estoqueService.listar()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (produtos) => {
          this.produtos = produtos;
        },
        error: (error) => {
          this.errorService.mostrarErro('Erro ao carregar produtos');
        }
      });
  }
}
