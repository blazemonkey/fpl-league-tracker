import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';

@Component({
  selector: 'team-cell',
  template: `<div class="image-container"><img width="20" [src]="imgForTeam" /></div>`,
  styles: [`
    .image-container {
      display: flex;
      align-items: center;
      height: 100%;
    }`]
})
export class TeamRenderer implements ICellRendererAngularComp {
  private teamBadge!: string;
  public imgForTeam!: string;

  agInit(params: ICellRendererParams): void {
    this.setTeamBadge(params);
  }

  refresh(params: ICellRendererParams): boolean {
    this.setTeamBadge(params);
    return true;
  }

  private setTeamBadge(params: ICellRendererParams) {
    this.teamBadge = params.value;
    this.imgForTeam = `https://resources.premierleague.com/premierleague/badges/50/t${this.teamBadge}@x2.png`;
  }
}