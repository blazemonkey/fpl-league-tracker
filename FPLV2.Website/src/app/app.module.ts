import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';

import { CarouselModule } from '@coreui/angular';
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

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'search-results', component: SearchResultsComponent },
  { path: 'season/:seasonId/league/:leagueId', component: LeagueComponent }
]

@NgModule({
  declarations: [
    AppComponent,
    SearchResultsComponent,
    HomeComponent,
    TitleComponent,
    LeagueComponent,
    StatTableComponent,
    ChartComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    AgGridModule,
    CarouselModule,
    FormsModule,
    RouterModule.forRoot(routes),
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
