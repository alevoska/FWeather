import { Component, Inject, OnInit, Input, OnChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-forecast',
  templateUrl: './forecast.component.html',
  styleUrls: ['./forecast.component.css']
})
export class ForecastComponent implements OnChanges {
  @Input('locationId') locationId = 1;
  private forecasts: Item[];
  private fromDate: string = '2022-01-01';
  private thruDate: string = '2022-12-31';
  private newDate: string;
  private newTemperature: string = '0';
  private newPrecipitation: string = '0.0';
  private newWindSpeed: string = '0';
  private newWindDir: string = '0';
  private graphFromDate: string;
  private graphThruDate: string;
  private graphPoints = [];

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  ngOnChanges(): void {
    this.fetchLocation();
  }

  fetchLocation() {
    console.log(`Fetching data for location ${this.locationId}`);
    this.http.get<Item[]>(this.baseUrl + 'weather/' + this.locationId).subscribe(result => {
      this.forecasts = result;
      console.log(`Fetch successful for location ${this.locationId}`);
      this.generateGraph();
    }, error => console.error(error));
  }

  createItem() {
    console.log(`Creating item ${this.newDate} to location ${this.locationId}`);
    this.http.post<Item>(this.baseUrl + 'weather/' + this.locationId + '/' + this.newDate, {
      temperature: this.newTemperature,
      precipitation: this.newPrecipitation,
      windStrength: this.newWindSpeed,
      windDirection: this.newWindDir
    }).subscribe(result => {
      this.forecasts = null;
      this.fetchLocation();
    }, error => console.error(error));

  }

  deleteItem(event, date) {
    console.log(`Deleting ${date} from location ${this.locationId}`);
    this.http.delete(this.baseUrl + 'weather/' + this.locationId + '/' + date).subscribe(result => {
      this.forecasts = null;
      this.fetchLocation();
    }, error => console.error(error));
  }

  regenerateData() {
    console.log(`Regenerating data for ${this.locationId}`);
    this.forecasts = null;
    this.http.get<Item[]>(this.baseUrl + 'weather/regenerate/' + this.locationId).subscribe(result => {
      this.fetchLocation();
    }, error => console.error(error));
  }

  generateGraph() {
    let dated = [];
    for (let item of this.forecasts) {
      dated[item.date] = item;
    }

    let from: Date, thru: Date;
    try {
      from = new Date(this.fromDate);
      thru = new Date(this.thruDate);
    } catch (error) {
      alert("Invalid date format. (Use YYYY-MM-DD)");
      console.error(error);
      return;
    }
    const timeBetween = thru.getTime() - from.getTime();
    const daysBetween = Math.ceil(timeBetween / (1000 * 3600 * 24)) + 1;

    if (daysBetween > 3650) {
      alert("Date range should be less than 10 years.");
      return;
    } else if (daysBetween < 2) {
      alert("Invalid date range.");
    }

    let points = [];
    let currDate = from.getTime();
    for (let i = 0; i < daysBetween; i++) {
      let newDate = new Date(currDate);
      let dateString = newDate.toISOString().slice(0, 10); // YYYY-MM-DD format
      if (typeof dated[dateString] !== 'undefined') {
        let temperature = dated[dateString].temperature;
        let color = (temperature >= 0) ? 'red' : 'navy';
        points.push({
          x: (i / daysBetween) * 1000,
          y: temperature,
          color: color
        });
      }
      currDate += 1000 * 3600 * 24; // Add 1 day
    }

    this.graphFromDate = this.fromDate;
    this.graphThruDate = this.thruDate;
    this.graphPoints = points;
  }
}

interface Item {
  date: string;
  location: string;
  temperature: number;
  precipitation: number;
  windStrength: number;
  windDirection: number;
}