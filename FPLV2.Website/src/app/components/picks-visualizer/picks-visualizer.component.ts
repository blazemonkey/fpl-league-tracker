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
  filteredPlayers: any[] = [];
  selectedElementTypes: number[] | undefined;
  filteredElementTypes: any[] = [];
  selectedTeam: number | undefined;
  filteredTeams: any[] = [];
  showCaptainsOnly: boolean = false;
  ignoreElementsWithNoPicks: boolean = false;

  columns: any[] = [ 
    { headerName: '', field: 'image', width: 40, cellRendererSelector: () => { return { component: TeamRenderer }; }, pinned: 'left' },
    { headerName: '', field: 'name', width: 200, pinned: 'left', cellStyle: { 'display': 'flex', 'height': '100%', 'align-items': 'center' } },
    { headerName: '', field: 'typeText', width: 60, pinned: 'left', cellStyle: { 'display': 'flex', 'height': '100%', 'align-items': 'center' } },
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

        this.filteredPlayers = this.players.sort((a, b) => a.teamName.localeCompare(b.teamName));
        this.selectedPlayers = this.filteredPlayers.map(x => x.id);              
    
        this.teams.unshift({ code: 0, name: 'All' });
        this.filteredTeams = this.teams;
        this.selectedTeam = 0;

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
      IgnoreElementsWithNoPicks: this.ignoreElementsWithNoPicks,
      ShowCaptainsOnly: this.showCaptainsOnly,
      PlayerIds: this.selectedPlayers,
      ElementTypes: this.selectedElementTypes,
      TeamId: this.selectedTeam
    }

    this.httpService.getPicks(this.seasonId, this.leagueId, options).subscribe({ 
      next: (values: any) =>
        {
          if (!values?.length)
            return;

          this.players.forEach((value, index) => {
              this.playersColorCode[value.id] = this.colors[index];
          });

          this.rows = values.map((x: { teamCode: string; firstName: string; secondName: string; elementTypeId: number, elementTypeText: string; totalPoints: number; values: any[]; }) => {
            const row: TableRow = {
              image: x.teamCode,
              name: `${x.firstName} ${x.secondName}`,
              typeId: x.elementTypeId,
              typeText: x.elementTypeText,
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

          var types = this.rows.map(x => {
            return {
                typeId: x.typeId,
                typeText: x.typeText
            }}).filter((value, index, self) => 
              index === self.findIndex((t) => (
                  t.typeId === value.typeId && t.typeText === value.typeText
              ))
          ).sort((a, b) => a.typeId - b.typeId);
          
          if (this.filteredElementTypes.length == 0) {
            this.filteredElementTypes = types;
          }
          if (this.selectedElementTypes == undefined) {
            this.selectedElementTypes = this.filteredElementTypes.map(x => x.typeId);
          }
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