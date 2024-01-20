import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PicksVisualizerComponent } from './picks-visualizer.component';

describe('PicksVisualizerComponent', () => {
  let component: PicksVisualizerComponent;
  let fixture: ComponentFixture<PicksVisualizerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PicksVisualizerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PicksVisualizerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
