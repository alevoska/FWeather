import { Component, EventEmitter, Inject, Input, Output } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-locations',
  templateUrl: './locations.component.html',
  styleUrls: ['./locations.component.css']
})
export class LocationsComponent {
  @Output() selected = new EventEmitter<number>();
  private locations: Location[];
  private newLocationInput: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.fetchLocations();
  }

  selectLocation(id: number) {
    this.selected.emit(id);
  }

  fetchLocations() {
    this.http.get<Location[]>(this.baseUrl + 'weather').subscribe(result => {
      this.locations = result;
    }, error => console.error(error));
  }

  createLocation() {
    console.log(`Creating item location`);
    this.http.post<Location>(this.baseUrl + 'weather', { title: this.newLocationInput }).subscribe(result => {
      this.fetchLocations();
    }, error => console.error(error));
  }

  deleteLocation(id: number) {
    this.http.delete(this.baseUrl + 'weather/' + id).subscribe(result => {
      this.locations = null;
      this.fetchLocations();
    }, error => console.error(error));
  }
}

interface Location {
  locationId?: number;
  title: string;
}