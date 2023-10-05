import { Component, OnInit } from '@angular/core';
import { TotalEnergyProductionFormComponent } from './totalEnergyProductionForm/totalEnergyProductionForm.component';
import { TotalEnergyProductionTableComponent } from './totalEnergyProductionTable/totalEnergyProductionTable.component';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { TotalEnergyProductionChartsComponent } from './totalEnergyProductionCharts/totalEnergyProductionCharts.component';
import { TotalEnergyProductionService } from './service/totalEnergyProduction.service';

@Component({
  selector: 'app-totalEnergyProduction',
  templateUrl: './totalEnergyProduction.component.html',
  styleUrls: ['./totalEnergyProduction.component.scss'],
  standalone : true,
  imports: [TotalEnergyProductionFormComponent, TotalEnergyProductionTableComponent, MatIconModule, MatTabsModule, 
    TotalEnergyProductionChartsComponent, CommonModule]
})
export class TotalEnergyProductionComponent implements OnInit {
  showError:boolean=false;
  constructor(public totalEnergyProductionService: TotalEnergyProductionService) { }

  ngOnInit() {
  }
  setError(value:boolean){
    this.showError = !value
  }

}
