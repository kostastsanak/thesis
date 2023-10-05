import { Component, OnInit } from '@angular/core';
import { BarChartModel } from 'src/app/core/models/charts.model';
import { BarChartComponent } from 'src/app/shared/standalone-components/bar-chart/bar-chart.component';
import { Consume_energyService } from '../service/consume_energy.service';
import { BehaviorSubject, Subscription } from 'rxjs';
import { ConsumeEnergyRequest, Consumption } from '../model/consumeEnergy.model';

@Component({
  selector: 'app-consumeEnergyBarChart',
  templateUrl: './consumeEnergyBarChart.component.html',
  styleUrls: ['./consumeEnergyBarChart.component.scss'],
  imports: [BarChartComponent],
  standalone: true
})
export class ConsumeEnergyBarChartComponent implements OnInit {

  consumeEnergyRequest$: BehaviorSubject<ConsumeEnergyRequest> = new BehaviorSubject(null);
  consumeEnergyRequest: ConsumeEnergyRequest;
  chartData: any[] = [];
  barChartModel$: BehaviorSubject<BarChartModel[]> = new BehaviorSubject([]);
  barChartModel: BarChartModel;

  subscriptions: Subscription = new Subscription();
  constructor(private consume_energyService: Consume_energyService) { }

  ngOnInit() {
    this.setRequest();
    this.requestGlobalSearch();
  }

  setRequest() {
    this.subscriptions.add(
      this.consume_energyService.consumeEnergyRequestSubject$.subscribe((data) => {
        if (data) {
          this.consumeEnergyRequest = data;
          this.consumeEnergyRequest = { ...this.consumeEnergyRequest, groupByCity: 1, noPagging: 1};//limit: 50, page: 0
          this.consumeEnergyRequest$.next(this.consumeEnergyRequest);
        }
      })
    );
  }

  requestGlobalSearch() {
    this.subscriptions.add(
      this.consumeEnergyRequest$.subscribe((data1) => {
        this.subscriptions.add(
          this.consume_energyService
            .consumeEnergySearch(this.consumeEnergyRequest)
            .subscribe((data) => {
              if (data && data.payload.consumptions) {
                this.chartData = this.fixChartData(data.payload.consumptions);
                this.barChartModel$.next(this.chartData);
              }
            })
        );
      })
    );
  }

  fixChartData(datas: Consumption[]): BarChartModel[] {
    let dataChartModel: BarChartModel[] = [];
    datas.forEach((data) => {
      let tmp: BarChartModel = {
        name: "",
        extra: "",
        value: 0,
      };
      tmp.name = data.area;
      tmp.value = data.energy_mwh;
      tmp.extra = data.area;
      dataChartModel.push(tmp);
    });
    return dataChartModel;
  }
  
  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
