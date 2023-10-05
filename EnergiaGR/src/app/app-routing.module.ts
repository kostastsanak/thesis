import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { DefaultComponent } from "./shared/standalone-components/default/default.component";

const routes: Routes = [
  {
    path: "",
    component: DefaultComponent,
    children: [
      {
        path: "home",
        loadComponent: () =>
          import("./features/datasets/datasets.component").then(
            (m) => m.DatasetsComponent
          )
      },
      {
        path: "globalSearch",
        loadComponent: () =>
          import("./features/globalSearch/globalSearch.component").then(
            (m) => m.GlobalSearchComponent
          )
      },
      {
        path: "consumeEnergy",
        loadComponent: () =>
          import("./features/consumeEnergy/consumeEnergy.component").then(
            (m) => m.ConsumeEnergyComponent
          )
      },
      {
        path: "totalEnergyProduction",
        loadComponent: () =>
          import("./features/totalEnergyProduction/totalEnergyProduction.component").then(
            (m) => m.TotalEnergyProductionComponent
          )
      },
      {
        path: "renewableEnergy",
        loadComponent: () =>
          import("./features/totalEnergyProduction/totalEnergyProduction.component").then(
            (m) => m.TotalEnergyProductionComponent
          )
      }
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
