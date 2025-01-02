import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountsService } from '../_services/accounts.service';
import { ToastrService } from 'ngx-toastr';

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
  private toaster=inject(ToastrService);
  model: any = {};

  register() {
   this.accountService.register(this.model).subscribe({
    next:response=>{
      console.log(response);
      this.cancel();
    },
    error: error=>this.toaster.error(error.error)
   });
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
