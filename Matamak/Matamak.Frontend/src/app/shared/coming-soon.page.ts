import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-coming-soon-page',
  imports: [RouterLink],
  template: `
    <main class="coming-soon" dir="rtl">
      <section>
        <p>Coming Soon</p>
        <h1>هذه الواجهة ستُبنى في المرحلة التالية</h1>
        <span>لن يتم استخدام أي API وهمي. سنربط كل شاشة هنا فقط عندما ننفذ ميزتها من الخطة المعتمدة.</span>
        <a routerLink="/auth/login">العودة إلى تسجيل الدخول</a>
      </section>
    </main>
  `,
  styles: `
    .coming-soon {
      display: grid;
      min-height: 100dvh;
      place-items: center;
      padding: 2rem;
      background: var(--matamak-surface);
    }

    section {
      width: min(100%, 42rem);
      border: 1px solid var(--matamak-border);
      border-radius: 0.5rem;
      background: #fff;
      box-shadow: var(--matamak-shadow);
      padding: clamp(2rem, 5vw, 4rem);
      text-align: center;
    }

    p {
      margin: 0 0 0.75rem;
      color: var(--matamak-accent);
      font-weight: 900;
    }

    h1 {
      margin: 0;
      color: var(--matamak-text);
      font-size: clamp(2rem, 5vw, 3.5rem);
      line-height: 1.2;
    }

    span {
      display: block;
      margin: 1rem auto 2rem;
      color: var(--matamak-text-muted);
      line-height: 1.8;
    }

    a {
      display: inline-flex;
      border-radius: 999px;
      background: var(--matamak-brown);
      color: #fff;
      padding: 0.9rem 1.4rem;
      text-decoration: none;
    }
  `
})
export class ComingSoonPage {}
