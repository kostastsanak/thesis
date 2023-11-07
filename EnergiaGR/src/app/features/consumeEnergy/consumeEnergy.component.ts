import { Component, OnInit } from '@angular/core';
import { ConsumeEnergyFormComponent } from './consumeEnergyForm/consumeEnergyForm.component';
import { ConsumeEnergyTableComponent } from './consumeEnergyTable/consumeEnergyTable.component';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { Consume_energyService } from './service/consume_energy.service';
import { ConsumeEnergyBarChartComponent } from './consumeEnergyBarChart/consumeEnergyBarChart.component';
import { ConsumeEnergyPieChartComponent } from './consumeEnergyPieChart/consumeEnergyPieChart.component';

@Component({
  selector: 'app-consumeEnergy',
  templateUrl: './consumeEnergy.component.html',
  styleUrls: ['./consumeEnergy.component.scss'],
  standalone : true,
  imports: [ConsumeEnergyTableComponent, ConsumeEnergyPieChartComponent,ConsumeEnergyFormComponent, ConsumeEnergyBarChartComponent, MatIconModule, MatTabsModule, CommonModule]
})
export class ConsumeEnergyComponent implements OnInit {
  showError:boolean=false;
  constructor(public consume_energyService: Consume_energyService) { }

  ngOnInit() {
  }
  setError(value:boolean){
    this.showError = !value
  }

}
