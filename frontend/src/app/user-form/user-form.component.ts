import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatError } from '@angular/material/form-field';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.scss']
})
export class UserFormComponent {
  user = {
    ssn: null,
    name: '',
    age: null,
    weight: null
  };
  errorMessage: string | null = null;

  onSubmit() {
    if (!this.user.ssn || !this.user.name || !this.user.age || !this.user.weight) {
      this.errorMessage = 'Bitte alle Felder ausfüllen!';
      return;
    }
    this.errorMessage = null;
    // Hier kannst du die User-Daten an das Backend senden
    // alert('User erstellt: ' + JSON.stringify(this.user));
  }
}
