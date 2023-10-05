import { Component, Input, OnInit } from '@angular/core';
import { BarChartComponent } from 'src/app/shared/standalone-components/bar-chart/bar-chart.component';
import { RoseDiagramComponent } from 'src/app/shared/standalone-components/rose-diagram/rose-diagram.component';
import { BehaviorSubject, Subscription } from 'rxjs';
import { TotalEnergyProductionRequest, Production } from '../model/totalEnergyProduction.model';
import { BarChartModel } from 'src/app/core/models/charts.model';
import { TotalEnergyProductionService } from '../service/totalEnergyProduction.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-totalEnergyProductionCharts',
  templateUrl: './totalEnergyProductionCharts.component.html',
  styleUrls: ['./totalEnergyProductionCharts.component.scss'],
  imports: [BarChartComponent,RoseDiagramComponent,CommonModule],
  standalone:true,
})
export class TotalEnergyProductionChartsComponent implements OnInit {
  @Input() type:string=""
  totalEnergyProductionRequest$: BehaviorSubject<TotalEnergyProductionRequest> = new BehaviorSubject(null);
  totalEnergyProductionRequest: TotalEnergyProductionRequest;
  chartData: any[] = [];
  barChartModel$: BehaviorSubject<BarChartModel[]> = new BehaviorSubject([]);
  barChartModel: BarChartModel;
  subscriptions: Subscription = new Subscription();
  
  constructor(private totalEnergyProductionService: TotalEnergyProductionService) { }

  ngOnInit() {
    this.setRequest();
    this.requestGlobalSearch();
  }

  setRequest() {
    this.subscriptions.add(
      this.totalEnergyProductionService.totalEnergyProductionRequestSubject$.subscribe((data) => {
        if (data) {
          this.totalEnergyProductionRequest = data;
          this.totalEnergyProductionRequest = { ...this.totalEnergyProductionRequest, groupByFuel: 1, noPagging: 1};//limit: 50, page: 0
          this.totalEnergyProductionRequest$.next(this.totalEnergyProductionRequest);
        }
      })
    );
  }

  requestGlobalSearch() {
    this.subscriptions.add(
      this.totalEnergyProductionRequest$.subscribe((data1) => {
        this.subscriptions.add(
          this.totalEnergyProductionService
            .totalEnergyProductionSearch(this.totalEnergyProductionRequest)
            .subscribe((data) => {
              if (data && data.payload.productions) {
                this.chartData = this.fixChartData(data.payload.productions);
                this.barChartModel$.next(this.chartData);
              }
            })
        );
      })
    );
  }

  fixChartData(datas: Production[]): BarChartModel[] {
    let dataChartModel: BarChartModel[] = [];
    datas.forEach((data) => {
      let tmp: BarChartModel = {
        name: "",
        extra: "",
        value: 0,
      };
      tmp.name = data.fuel;
      tmp.value = data.energy_mwh;
      tmp.extra = data.fuel;
      dataChartModel.push(tmp);
    });
    return dataChartModel;
  }
  
  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

}
