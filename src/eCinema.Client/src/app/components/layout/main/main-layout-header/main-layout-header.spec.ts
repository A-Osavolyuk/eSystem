import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MainLayoutHeader } from './main-layout-header';

describe('MainLayoutHeader', () => {
  let component: MainLayoutHeader;
  let fixture: ComponentFixture<MainLayoutHeader>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MainLayoutHeader]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MainLayoutHeader);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
