import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  template: `
    <div class="app-container">
      <header class="app-header">
        <h1>🧾 KORP - Sistema de Emissão de Notas Fiscais</h1>
      </header>
      <nav class="app-nav">
        <a routerLink="/produtos">Produtos</a>
        <a routerLink="/notas-fiscais">Notas Fiscais</a>
      </nav>
      <main class="app-main">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [`
    .app-container {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
      background-color: #f5f5f5;
    }

    .app-header {
      background-color: #2c3e50;
      color: white;
      padding: 20px;
      text-align: center;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .app-header h1 {
      margin: 0;
      font-size: 28px;
    }

    .app-nav {
      background-color: #34495e;
      padding: 0;
      display: flex;
      gap: 20px;
      padding-left: 20px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .app-nav a {
      color: white;
      text-decoration: none;
      padding: 15px 20px;
      cursor: pointer;
      transition: background-color 0.3s ease;
      font-weight: 500;
    }

    .app-nav a:hover {
      background-color: #2c3e50;
    }

    .app-main {
      flex: 1;
      padding: 20px;
      max-width: 1200px;
      margin: 0 auto;
      width: 100%;
    }
  `]
})
export class AppComponent {
  title = 'korp-frontend';
}
