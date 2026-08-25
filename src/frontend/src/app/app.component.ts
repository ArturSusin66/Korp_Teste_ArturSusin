import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterOutlet, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="app-container">
      <header class="app-header">
        <h1>📑 KORP - Sistema de Emissão de Notas Fiscais</h1>
      </header>
      <nav class="app-nav">
        <a routerLink="/produtos" routerLinkActive="active">Produtos</a>
        <a routerLink="/notas-fiscais" routerLinkActive="active">Notas Fiscais</a>
      </nav>
      <main class="app-main">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [`
    .app-container {
      font-family: Arial, sans-serif;
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }
    .app-header {
      margin-bottom: 20px;
    }
    .app-nav {
      display: flex;
      gap: 15px;
      margin-bottom: 20px;
      border-bottom: 2px solid #ccc;
      padding-bottom: 10px;
    }
    .app-nav a {
      text-decoration: none;
      color: #0066cc;
      font-weight: bold;
      padding: 5px 10px;
      border-radius: 4px;
    }
    .app-nav a.active {
      background-color: #0066cc;
      color: white;
    }
  `]
})
export class AppComponent {
  title = 'korp-frontend';
}