import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AgGridModule } from 'ag-grid-angular'
import { AppComponent } from './app.component';
import { TeamRenderer } from './team-renderer.component'
import { PickRenderer } from './pick-renderer.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AppComponent,    
    PickRenderer,
    TeamRenderer
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AgGridModule,
    FormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
