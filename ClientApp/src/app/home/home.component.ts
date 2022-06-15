import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  locationId = 1;

  onSelected(id: number) {
    console.log(`Selected location ${id}`);
    this.locationId = id;
  }
}
