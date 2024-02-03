import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../config'


@Injectable({
  providedIn: 'root'
})
export class HttpService {
  constructor(private http: HttpClient) {}  

  getCurrentSeason() {
    var url = `${environment.apiUrl}/seasons/latest`;
    return this.get(url);
  }

  searchLeagues(seasonId: number, name: string) {
    var url = `${environment.apiUrl}/leagues/search/${seasonId}`;
    var params = new HttpParams().set('leagueIdOrName', name);
    return this.get<any[]>(url, params);
  }

  getLeagueSummary(seasonId: number, leagueId: number) {
    var url = `${environment.apiUrl}/leagues/${seasonId}/${leagueId}/summary`;
    return this.get(url);
  }

  getStats() {
    var url = `${environment.apiUrl}/stats`;
    return this.get(url);
  }

  getStat(type: number, statId: number, seasonId: number, leagueId: number, playerId: number = 0) {
    var url = '';
    if (type == 1)
      url = `${environment.apiUrl}/stats/overall/${statId}/${seasonId}/${leagueId}`;
    else if (type == 2)
      url = `${environment.apiUrl}/stats/team/${statId}/${seasonId}/${leagueId}/${playerId}`;
    
    return this.get(url);
  }

  getCharts() {
    var url = `${environment.apiUrl}/charts`;
    return this.get(url);
  }

  getChart(type: number, chartId: number, seasonId: number, leagueId: number) {
    var typeName = type == 1 ? 'line' : '';
    var url = `${environment.apiUrl}/charts/${typeName}/${chartId}/${seasonId}/${leagueId}`;
    return this.get(url);
  }

  getPicks(seasonId: number, leagueId: number, options: any) {
    var url = `${environment.apiUrl}/charts/points/${seasonId}/${leagueId}`;
    return this.post(url, options);
  }

  private get<T>(url: string, params?: HttpParams) {
    return this.http.get<T>(url, { params: params });
  }

  private post(url: string, data: any) {
    return this.http.post(url, data);
  }
}