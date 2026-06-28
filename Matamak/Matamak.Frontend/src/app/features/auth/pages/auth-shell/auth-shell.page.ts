import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-auth-shell',
  imports: [RouterLink, RouterOutlet],
  templateUrl: './auth-shell.page.html',
  styleUrl: './auth-shell.page.scss'
})
export class AuthShellPage {}
