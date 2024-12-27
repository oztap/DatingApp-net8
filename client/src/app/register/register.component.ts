import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountsService } from '../_services/accounts.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private accountService=inject(AccountsService);
  cancelRegister=output<boolean>();
  model: any = {};

  register() {
   this.accountService.register(this.model).subscribe({
    next:response=>{
      console.log(response);
      this.cancel();
    }
   });
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
