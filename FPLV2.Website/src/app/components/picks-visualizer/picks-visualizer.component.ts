import { Component, ElementRef, Input, SimpleChanges, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RowHeightParams } from 'ag-grid-community';
import { HttpService } from 'src/app/services/http.service';
import { TeamRenderer } from './renderers/team-renderer.component';
import { PickRenderer } from './renderers/pick-renderer.component';

@Component({
  selector: 'app-picks-visualizer',
  templateUrl: './picks-visualizer.component.html',
  styleUrl: './picks-visualizer.component.scss'
})
export class PicksVisualizerComponent {
  @Input() seasonId: number = 0;
  @Input() leagueId: number = 0;
  @Input() players: any[] = [];
  @Input() teams: any[] = [];

  selectedPlayers: number[] | undefined;
  filterPlayers: any[] = [];

  columns: any[] = [ 
    { headerName: '', field: 'image', width: 40, cellRendererSelector: () => { return { component: TeamRenderer }; }, pinned: 'left' },
    { headerName: '', field: 'name', width: 200, pinned: 'left', cellStyle: { 'display': 'flex', 'height': '100%', 'align-items': 'center' } },
    { headerName: '', field: 'type', width: 60, pinned: 'left', cellStyle: { 'display': 'flex', 'height': '100%', 'align-items': 'center' } },
    { headerName: '', field: 'totalPoints', width: 60, pinned: 'left', cellStyle: { 'display': 'flex', 'height': '100%', 'align-items': 'center','font-weight': 'bold' } }];
  rows: any[] = [];
  headerHeight = '';
  headerWidth = '';

  colors: string[] = ["#396ab1","#da7c30","#3e9651","#cc2529","#f8518a","#7d26cd"];
  playersColorCode:  { [key: string]: string } = {};  
  fullScreen: boolean = false;
  
  constructor(private route: ActivatedRoute, private httpService: HttpService, private router: Router) {
    for (var i = 1; i <= 38; i++) {
      this.columns.push({ headerName: i, field: `gameweek${i}`, width: 55, cellRendererSelector: () => { return { component: PickRenderer }; } })
    }

    this.route.paramMap.subscribe(params => {
      var seasonIdParam = params.get('seasonId');
      if (seasonIdParam)
        this.seasonId = +seasonIdParam;

      var leagueIdParam = params.get('leagueId');
      if (leagueIdParam)
        this.leagueId = +leagueIdParam;

      var navigation = this.router.getCurrentNavigation();
      if (navigation && navigation.extras.state)
      {
        this.players = navigation.extras.state['players'];
        this.teams = navigation.extras.state['teams'];

        this.filterPlayers = this.players.sort((a, b) => a.teamName.localeCompare(b.teamName));
        this.selectedPlayers = this.filterPlayers.map(x => x.id);
        
        this.fullScreen = true;
        this.getPicks();
      }
    });
  }  

  ngOnChanges(changes: SimpleChanges): void {
    this.getPicks();    
  }

  getPicks() {
    var options = {
      IgnoreElementsWithNoPicks: false,
      ShowCaptainsOnly: false,
      PlayerIds: this.selectedPlayers
    }

    this.httpService.getPicks(this.seasonId, this.leagueId, options).subscribe({ 
      next: (values: any) =>
        {
          if (!values?.length)
            return;

          this.players.forEach((value, index) => {
              this.playersColorCode[value.id] = this.colors[index];
          });

          this.rows = values.map((x: { teamCode: any; firstName: any; secondName: any; elementType: number; totalPoints: any; values: any[]; }) => {
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
      complete: () => 
        {  
          this.headerHeight = document.getElementsByClassName("ag-header")[0]?.clientHeight + "px";
          this.headerWidth = document.getElementsByClassName("ag-header-row")[0]?.clientWidth + "px";
        }});    
  }

  getRowHeight(params: RowHeightParams): number | undefined | null {
    var maxPicks = params.data.maxPicks;
    return maxPicks == 0 ? 35 : (55 + (16 * (maxPicks - 1)));
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
      this.rows = [];
      this.getPicks();
    }
  }
}

interface TableRow {
  image: any;
  name: string;
  totalPoints: any;
  [key: string]: any; // Allow additional properties with any type
}