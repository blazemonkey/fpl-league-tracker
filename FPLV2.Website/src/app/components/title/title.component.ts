import { Component, EventEmitter, Output } from '@angular/core';
import { HttpService } from 'src/app/services/http.service';

@Component({
  selector: 'app-title',
  templateUrl: './title.component.html',
  styleUrls: ['./title.component.scss']
})
export class TitleComponent {
  title = 'FPL Visualized';
  year = '';

  @Output() seasonIdEvent = new EventEmitter<number>();

  constructor(private httpService: HttpService) {

  }

  ngOnInit() {
    this.httpService.getCurrentSeason().subscribe({next: (values: any) => 
      { 
        this.year = values.year;
        this.seasonIdEvent.emit(values.id);
      }
    });
  }
}
