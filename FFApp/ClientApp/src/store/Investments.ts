import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';
// -----------------
// STATE - This defines the type of data maintained in the Redux store.

enum DisplayType {
    PORTFOLIO_GROUPING = 1,
    INVESTMENT = 2
}

enum ActionType {
    REQUEST_INVESTMENTS = 'REQUEST_INVESTMENTS',
    REQUEST_INVESTMENTS_SUCCESS = 'REQUEST_INVESTMENTS_SUCCESS',
    REQUEST_INVESTMENTS_ERROR = 'REQUEST_INVESTMENTS_ERROR',
}

export interface InvestmentState {
    isLoading: boolean;
    error: string | null;
    investments: InvestmentBreakDown[];
}

export interface ChartData {
    label: string;
    value: string;
}

export interface InvestmentBreakDown {
    id: string;
    label: string;
    value: number;
    displayOrder: number;
    displayType: DisplayType;
    linkId: string;
    hierachyLevel: number;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestInvestmentsAction {
    type: ActionType.REQUEST_INVESTMENTS;
}

interface RequestInvestmentsSuccessAction {
    type: ActionType.REQUEST_INVESTMENTS_SUCCESS;
    payload: InvestmentBreakDown[];
}

interface RequestInvestmentsErrorAction {
    type: ActionType.REQUEST_INVESTMENTS_ERROR;
    payload: string;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction =
    | RequestInvestmentsAction
    | RequestInvestmentsSuccessAction
    | RequestInvestmentsErrorAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.

export const actionCreators =  {
    requestInvestments: (): AppThunkAction<KnownAction> => (dispatch) => {
        dispatch({ type: ActionType.REQUEST_INVESTMENTS });

        try {
            fetch(`portfolio`)
            .then(response => response.json() as Promise<InvestmentBreakDown[]>)
            .then(data => {
                dispatch({ type: ActionType.REQUEST_INVESTMENTS_SUCCESS, payload: data });
            });
        } catch (err) {
            dispatch({ type: ActionType.REQUEST_INVESTMENTS_ERROR, payload: err.message });
        }
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: InvestmentState = { investments: [], isLoading: false, error: null };

export const reducer: Reducer<InvestmentState> = (state: InvestmentState | undefined, incomingAction: Action): InvestmentState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case ActionType.REQUEST_INVESTMENTS:
            return {
                isLoading: true,
                investments: [],
                error: null,
            };
        case ActionType.REQUEST_INVESTMENTS_ERROR:
            return {
                isLoading: false,
                investments: [],
                error: action.payload
            };
        case ActionType.REQUEST_INVESTMENTS_SUCCESS:
            return {
                isLoading: false,
                investments: action.payload,
                error: null
            };
        default:
            return state;
    }
};
