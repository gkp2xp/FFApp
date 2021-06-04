import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';
import { ApplicationState } from '../store';
import * as InvestmentsStore from '../store/Investments';

import Chart from 'react-google-charts';

// At runtime, Redux will merge together...
type InvestmentBreakDownProps =
    InvestmentsStore.InvestmentState // ... state we've requested from the Redux store
    & typeof InvestmentsStore.actionCreators; // ... plus action creators we've requested

class InvestmentBreakDown extends React.Component<InvestmentBreakDownProps> {

  // This method is called when the component is first added to the document
  public componentDidMount() {
    this.ensureDataFetched();
  }

  public render() {
    return (
      <React.Fragment>
        <h1 id="tabelLabel">Investments</h1>
            {this.renderChart()}
            {this.renderTable()}

      </React.Fragment>
    );
  }

  private ensureDataFetched() {
      this.props.requestInvestments();
    }

    private renderChart() {
        if (this.props.error || this.props.isLoading) {
            return null;
        }

        const data: InvestmentsStore.ChartData[] = this.props.investments
            .filter((data: InvestmentsStore.InvestmentBreakDown) => data.hierachyLevel === 1)
            .map((data: InvestmentsStore.InvestmentBreakDown) => { return { label: data.label, value: data.value.toString() } });

        let temp: any = [];
        temp.push(["Grouping", "Value"]);
        data.forEach((item) => temp.push([item.label, parseInt(item.value)]));

        return (
             <Chart
                width={'500px'}
                height={'300px'}
                chartType="BarChart"
                loader={<div>Loading Chart</div>}
                data={temp}
                options={{
                    // Material design options
                    title: 'Top-level portfolio grouping',
                    chartArea: { width: '50%' },
                    hAxis: {
                        title: 'Investments',
                        minValue: 0,
                    },
                    vAxis: {
                        title: 'Top-level portfolio'
                    }
                }}
            />
        );
    }

  private renderTable() {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
                <tr>
            <th>Label</th>
                    <th><span style={{ float: 'right' }}>Value</span></th>
          </tr>
        </thead>
            <tbody>

                {this.props.isLoading && <h3>Loading...</h3>}
                {this.props.error && <h3>{this.props.error}</h3>}

                {!this.props.error && !this.props.isLoading && this.props.investments.map((investment: InvestmentsStore.InvestmentBreakDown) => 
                    <tr key={investment.id}>
                        <td><div style={{ marginLeft: (investment.hierachyLevel * 20).toString() + "px" }} >{investment.label}</div></td>
                        <td><div style={{ float: 'right' }}>{investment.value}</div></td>
                        <td><button>Projected Value</button></td>
                    </tr>
          )}
        </tbody>
      </table>
    );
  }

}

export default connect(
    (state: ApplicationState) => state.investments, // Selects which state properties are merged into the component's props
  InvestmentsStore.actionCreators // Selects which action creators are merged into the component's props
)(InvestmentBreakDown as any);
