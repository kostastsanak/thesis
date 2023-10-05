import { Injectable } from "@angular/core";

export interface NavItem {
  title: string;
  disabled?: boolean;
  icon?: string;
  route?: string;
  disallow?: string[];
  children?: NavItem[];
}

@Injectable({
  providedIn: "root",
})
export class SidebarService {
  constructor() {}

  menu: NavItem[] = [
    {
      title: "Home",
      icon: "home",
      route: "home",
    },
    {
      title: "Energy Consumption",
      icon: "electric_bolt",
      route: "consumeEnergy",
    },
    {
      title: "Total Energy Production",
      icon: "merge",
      route: "totalEnergyProduction",
    },
    {
      title: "Renewable Energy",
      icon: "recycling",
      route: "renewableEnergy",
    },
    
    

  ];
}
