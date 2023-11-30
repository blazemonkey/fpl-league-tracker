import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../config'


@Injectable({
  providedIn: 'root'
})
export class HttpService {
  constructor(private http: HttpClient) {}  

  getCurrentYear() {
    var url = `${environment.apiUrl}/seasons/latest`;
    return this.get(url);
  }

  searchLeagues(name: string) {
    var url = `${environment.apiUrl}/leagues/search`;
    var params = new HttpParams().set('name', name);
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

  getStat(type: number, statId: number, seasonId: number, leagueId: number) {
    var typeName = type == 1 ? 'overall' : '';
    var url = `${environment.apiUrl}/stats/${typeName}/${statId}/${seasonId}/${leagueId}`;
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

  private get<T>(url: string, params?: HttpParams) {
    return this.http.get<T>(url, { params: params });
  }

  private post(url: string, data: any) {
    return this.http.post(url, data);
  }
}