import { Component } from '@angular/core';
import { RegisterComponent } from '../register/register.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent  {
  registerMode = false;

  // ngOnInit(): void {
  //   this.getUsers();
  // }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }
  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }

  // getUsers() {
  //   this.http.get('http://localhost:5000/api/users').subscribe({
  //     next: (response) => (this.users = response),
  //     error: (error) => console.log(error),
  //     complete: () => console.log('Request has completed'),
  //   });
  // }
}
