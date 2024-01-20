import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';

import { CarouselModule } from '@coreui/angular';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SearchResultsComponent } from './components/search-results/search-results.component';
import { HomeComponent } from './components/home/home.component'
import { TitleComponent } from './components/title/title.component'
import { HttpClientModule } from '@angular/common/http';
import { AgGridModule } from 'ag-grid-angular';
import { LeagueComponent } from './components/league/league.component';
import { StatTableComponent } from './components/stat-table/stat-table.component';
import { ChartComponent } from './components/chart/chart.component';
import { PicksVisualizerComponent } from './components/picks-visualizer/picks-visualizer.component';
import { PickRenderer } from './components/picks-visualizer/renderers/pick-renderer.component';
import { TeamRenderer } from './components/picks-visualizer/renderers/team-renderer.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'search-results', component: SearchResultsComponent },
  { path: 'season/:seasonId/league/:leagueId', component: LeagueComponent },
  { path: 'season/:seasonId/league/:leagueId/chart/:type/:chartId', component: ChartComponent },
  { path: 'season/:seasonId/league/:leagueId/picks', component: PicksVisualizerComponent }
]

@NgModule({
  declarations: [
    AppComponent,
    SearchResultsComponent,
    HomeComponent,
    TitleComponent,
    LeagueComponent,
    StatTableComponent,
    ChartComponent,
    PicksVisualizerComponent,
    PickRenderer,
    TeamRenderer
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    AgGridModule,
    CarouselModule,
    NgxChartsModule,
    FormsModule,
    RouterModule.forRoot(routes),
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
