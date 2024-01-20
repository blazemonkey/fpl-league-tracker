import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';

@Component({
  selector: 'pick-cell',
  template: `
    <div class="picks-container">
      <div>{{ points }}</div>
        <div class="dot-container">
          <div *ngFor="let p of picks" class="points-dot" style="background-color:{{p.playerColourCode}}; border: {{(p.multiplier == 2 ? '1.5px' : p.multiplier == 3 ? '2px' : '0px')}} solid white; opacity: {{(p.multiplier == 0 ? '20%' : '100%')}}"></div>
        </div>
    </div>`,
  styles: [`
    .picks-container {
      text-align: center;
      line-height: 32px;
    }
    .dot-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      margin: 0px 0px 10px 0px;
    }    
    .points-dot {
      height: 10px;
      width: 10px;
      margin: 3px;
      border-radius: 50%;
      display: inline-block;
      box-sizing: border-box;
    }`]
})

export class PickRenderer implements ICellRendererAngularComp {
    params!: ICellRendererParams;
    points!: string;
    picks!: any[];
  
    agInit(params: ICellRendererParams): void {
      this.params = params;      
      this.setPick(params);
    }
  
    refresh(params: ICellRendererParams): boolean {
      this.params = params;
      this.setPick(params);
      return true;
    }
  
    private setPick(params: ICellRendererParams) {      
      this.points = params?.value?.points ?? '-';
      this.picks = params?.value?.picks;
    }
}