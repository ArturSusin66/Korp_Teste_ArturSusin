import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EstoqueService } from '../../shared/services/estoque.service';
import { ErrorService } from '../../shared/services/error.service';
import { Produto } from '../../shared/models/produto.model';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-produtos',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="container">
      <h2>📦 Gestão de Produtos</h2>

      <!-- Mensagens de Sucesso/Erro -->
      <div *ngIf="(errorService.error$ | async) as erro" class="alert alert-error">
        ❌ {{ erro }}
      </div>
      <div *ngIf="(errorService.success$ | async) as sucesso" class="alert alert-success">
        ✅ {{ sucesso }}
      </div>

      <!-- Formulário de Novo Produto -->
      <div class="form-section">
        <h3>Criar Novo Produto</h3>
        <form [formGroup]="produtoForm" (ngSubmit)="criarProduto()">
          <div class="form-group">
            <label for="codigo">Código:</label>
            <input
              id="codigo"
              type="text"
              formControlName="codigo"
              placeholder="Ex: P001"
              required
            />
          </div>

          <div class="form-group">
            <label for="descricao">Descrição:</label>
            <input
              id="descricao"
              type="text"
              formControlName="descricao"
              placeholder="Ex: Notebook Dell"
              required
            />
          </div>

          <div class="form-group">
            <label for="saldo">Saldo (Quantidade):</label>
            <input
              id="saldo"
              type="number"
              formControlName="saldo"
              placeholder="Ex: 10"
              min="0"
              required
            />
          </div>

          <button type="submit" [disabled]="produtoForm.invalid || carregando">
            {{ carregando ? 'Salvando...' : 'Salvar Produto' }}
          </button>
        </form>
      </div>

      <!-- Listagem de Produtos -->
      <div class="list-section">
        <h3>Produtos Cadastrados</h3>
        <div *ngIf="carregandoLista" class="loading">Carregando produtos...</div>
        <table *ngIf="!carregandoLista && produtos.length > 0" class="table">
          <thead>
            <tr>
              <th>Código</th>
              <th>Descrição</th>
              <th>Saldo</th>
              <th>Criado em</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let produto of produtos">
              <td>{{ produto.codigo }}</td>
              <td>{{ produto.descricao }}</td>
              <td class="saldo">{{ produto.saldo }}</td>
              <td>{{ produto.criadoEm | date: 'dd/MM/yyyy HH:mm' }}</td>
            </tr>
          </tbody>
        </table>
        <div *ngIf="!carregandoLista && produtos.length === 0" class="no-data">
          Nenhum produto cadastrado
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container {
      max-width: 900px;
      margin: 0 auto;
    }

    h2 {
      color: #2c3e50;
      margin-bottom: 20px;
    }

    h3 {
      color: #34495e;
      margin-top: 20px;
      margin-bottom: 15px;
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
      from {
        opacity: 0;
        transform: translateY(-10px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }

    .form-section, .list-section {
      background: white;
      padding: 20px;
      border-radius: 4px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      margin-bottom: 20px;
    }

    .form-group {
      margin-bottom: 15px;
      display: flex;
      flex-direction: column;
    }

    label {
      margin-bottom: 5px;
      font-weight: 600;
      color: #34495e;
    }

    input {
      padding: 10px;
      border: 1px solid #bdc3c7;
      border-radius: 4px;
      font-size: 14px;
    }

    input:focus {
      outline: none;
      border-color: #3498db;
      box-shadow: 0 0 5px rgba(52, 152, 219, 0.3);
    }

    button {
      padding: 10px 20px;
      background-color: #27ae60;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 14px;
      font-weight: 600;
      transition: background-color 0.3s ease;
    }

    button:hover:not(:disabled) {
      background-color: #229954;
    }

    button:disabled {
      background-color: #95a5a6;
      cursor: not-allowed;
    }

    .table {
      width: 100%;
      border-collapse: collapse;
      font-size: 14px;
    }

    .table thead {
      background-color: #ecf0f1;
      font-weight: 600;
      color: #2c3e50;
    }

    .table th, .table td {
      padding: 12px;
      text-align: left;
      border-bottom: 1px solid #bdc3c7;
    }

    .table tbody tr:hover {
      background-color: #f9f9f9;
    }

    .saldo {
      font-weight: 600;
      color: #27ae60;
    }

    .loading, .no-data {
      text-align: center;
      padding: 20px;
      color: #7f8c8d;
    }
  `]
})
export class ProdutosComponent implements OnInit, OnDestroy {
  produtoForm: FormGroup;
  produtos: Produto[] = [];
  carregando = false;
  carregandoLista = false;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private estoqueService: EstoqueService,
    public errorService: ErrorService
  ) {
    this.produtoForm = this.fb.group({
      codigo: ['', [Validators.required, Validators.minLength(1)]],
      descricao: ['', [Validators.required, Validators.minLength(3)]],
      saldo: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.carregarProdutos();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  criarProduto(): void {
    if (this.produtoForm.invalid) return;

    this.carregando = true;
    const { codigo, descricao, saldo } = this.produtoForm.value;

    this.estoqueService.criar(codigo, descricao, saldo)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.errorService.mostrarSucesso(`Produto "${descricao}" criado com sucesso!`);
          this.produtoForm.reset();
          this.carregarProdutos();
        },
        error: (error) => {
          const mensagem = error.error?.mensagem || 'Erro ao criar produto';
          this.errorService.mostrarErro(mensagem);
          this.carregando = false;
        }
      });
  }

  private carregarProdutos(): void {
    this.carregandoLista = true;
    this.estoqueService.listar()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (produtos) => {
          this.produtos = produtos;
          this.carregandoLista = false;
          this.carregando = false;
        },
        error: (error) => {
          const mensagem = error.error?.mensagem || 'Erro ao carregar produtos';
          this.errorService.mostrarErro(mensagem);
          this.carregandoLista = false;
          this.carregando = false;
        }
      });
  }
}
