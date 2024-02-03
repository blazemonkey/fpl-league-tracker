import { Component, OnInit } from '@angular/core';
import { Feature } from '../../models/feature';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  seasonId: number = 0;
  features: Array<Feature> = [];
  searchQuery: string = '';

  constructor(private router: Router) { }

  ngOnInit(): void {
    this.features.push(new Feature('../../assets/features/picks.png', 'Picks Visualizer', 'See the pattern of picks that players in your Mini League are making'))
    this.features.push(new Feature('../../assets/features/chart.png', 'Charts', 'See your Mini League represented as various charts'))
  }

  onSeasonLoaded(data: any): void {
    this.seasonId = data;
  }

  search(event: KeyboardEvent): void {
    if ((event.key != "Enter") || (!this.searchQuery))
      return;
    
    this.router.navigate(['/search-results'], { queryParams: { seasonId: this.seasonId, name: encodeURI(this.searchQuery) } });
  }
}
