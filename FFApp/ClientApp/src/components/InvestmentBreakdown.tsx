import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';
import { ApplicationState } from '../store';
import * as InvestmentsStore from '../store/Investments';


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
        {this.renderTable()}
      </React.Fragment>
    );
  }

  private ensureDataFetched() {
      this.props.requestInvestments();
  }

  private renderTable() {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
                <tr>
            <th>Label</th>
                    <th><span style={{ float: 'right' }}>Value</span></th>
                    <th>LinkId</th>
          </tr>
        </thead>
            <tbody>

                {this.props.isLoading && <h3>Loading...</h3>}
                {this.props.error && <h3>{this.props.error}</h3>}

                {!this.props.error && !this.props.isLoading && this.props.investments.map((investment: InvestmentsStore.InvestmentBreakDown) => 
                    <tr key={investment.id}>
                        <td><div style={{ marginLeft: (investment.hierachyLevel * 20).toString() + "px" }} >{investment.label}</div></td>
                        <td><div style={{ float: 'right' }}>{investment.value}</div></td>
                        <td><div>{investment.linkId}</div></td>
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
