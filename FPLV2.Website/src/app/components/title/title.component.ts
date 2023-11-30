import { Component } from '@angular/core';
import { HttpService } from 'src/app/services/http.service';

@Component({
  selector: 'app-title',
  templateUrl: './title.component.html',
  styleUrls: ['./title.component.scss']
})
export class TitleComponent {
  title = 'FPL Visualized';
  year = '';

  constructor(private httpService: HttpService) {

  }

  ngOnInit() {
    this.httpService.getCurrentYear().subscribe({next: (values: any) => { this.year = values.year }});
  }
}
