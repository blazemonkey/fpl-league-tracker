import { HttpClient } from '@angular/common/http';
import { Component, ViewChild, ElementRef  } from '@angular/core';
import { TeamRenderer } from './team-renderer.component';
import { ICellRendererParams, RowHeightParams } from 'ag-grid-community';
import { forkJoin } from 'rxjs';
import { PickRenderer } from './pick-renderer.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  constructor(private http: HttpClient) { } 
  title = 'FPLV2.WebsiteTest';
  loading: boolean = false;
  url: string = 'https://localhost:7027' //'https://fpl-v2-api.azurewebsites.net/';
  seasonId: number = 1//2;
  leagueId: number = 124141//164180;

  columns: any[] = [ 
  { headerName: '', field: 'image', width: 40, cellRendererSelector: () => { return { component: TeamRenderer }; }, pinned: 'left' },
  { headerName: '', field: 'name', width: 250, pinned: 'left', cellStyle: { 'display': 'flex', 'height': '100%', 'align-items': 'center' } },
  { headerName: '', field: 'type', width: 60, pinned: 'left', cellStyle: { 'display': 'flex', 'height': '100%', 'align-items': 'center' } },
  { headerName: '', field: 'totalPoints', width: 60, pinned: 'left', cellStyle: { 'display': 'flex', 'height': '100%', 'align-items': 'center','font-weight': 'bold' } }];
  rows: any[] = [];

  colors: string[] = ["#396ab1","#da7c30","#3e9651","#cc2529","#f8518a","#7d26cd"];
  playersColorCode:  { [key: string]: string } = {};
  teams: any[] = [];
  players: any[] = [];

  options = {
    IgnoreElementsWithNoPicks: false,
    ShowCaptainsOnly: false
  }

  ngOnInit() {        
    for (var i = 1; i <= 38; i++) {
      this.columns.push({ headerName: i, field: `gameweek${i}`, width: 55, cellRendererSelector: () => { return { component: PickRenderer }; } })
    }

    this.refresh();
  }

  refresh() {
    this.loading = true;
    var teamsReq = this.http.get<any[]>(`${this.url}/teams/${this.seasonId}`);   
    var playersReq = this.http.get<any[]>(`${this.url}/players/${this.seasonId}/${this.leagueId}`);   
    var pointsReq = this.http.post<any[]>(`${this.url}/charts/points/${this.seasonId}/${this.leagueId}`, this.options);

    var requests = forkJoin([teamsReq, playersReq, pointsReq]);
    requests.subscribe({
      next: values => {
        this.teams = values[0];
        this.players = values[1];
        var points = values[2];
        
        this.players.forEach((value, index) => {
            this.playersColorCode[value.id] = this.colors[index];
        });

        this.rows = points.map((x) => {
          const row: TableRow = {
            image: x.teamCode,
            name: `${x.firstName} ${x.secondName}`,
            type: x.elementType == 1 ? "GKP" : x.elementType == 2 ? "DEF" : x.elementType == 3 ? "MID" : "FWD",
            totalPoints: x.totalPoints,            
          };
        
          var maxPicks = 0;
          for (let i = 1; i <= 38; i++) {
            var gw = x.values.find((z: { gameweek: number; }) => z.gameweek === i);
            for (let j = 0; j < gw?.picks?.length ?? 0; j++) {
              gw.picks[j]['playerColourCode'] = this.playersColorCode[gw.picks[j].playerId];
            }
            if (gw?.picks?.length > maxPicks)
              maxPicks = gw.picks.length;

            row[`gameweek${i}`] = gw;
          }
        
          row['maxPicks'] = maxPicks;
          return row;
        });
      },
      complete: () => { this.loading = false; }
    });    
  }

  getRowHeight(params: RowHeightParams): number | undefined | null {
    var maxPicks = params.data.maxPicks;
    return maxPicks == 0 ? 35 : (55 + (16 * (maxPicks - 1)));
  }

  @ViewChild('infoDialog') infoDialog!: ElementRef;
  openInfoDialog() {
    const dialog: HTMLDialogElement | null = this.infoDialog.nativeElement;

    if (dialog) {
      dialog.showModal();
    }
  }

  closeInfoDialog() {
    const dialog: HTMLDialogElement | null = this.infoDialog.nativeElement;

    if (dialog) {
      dialog.close();
    }
  }

  @ViewChild('filterDialog') filterDialog!: ElementRef;
  openFilterDialog() {
    const dialog: HTMLDialogElement | null = this.filterDialog.nativeElement;

    if (dialog) {
      dialog.showModal();
    }
  }

  closeFilterDialog(apply: boolean) {
    const dialog: HTMLDialogElement | null = this.filterDialog.nativeElement;

    if (dialog) {
      dialog.close();
    }

    if (apply) {
      this.refresh();
    }
  }
}

interface TableRow {
  image: any;
  name: string;
  totalPoints: any;
  [key: string]: any; // Allow additional properties with any type
}