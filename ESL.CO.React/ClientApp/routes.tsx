import * as React from 'react';
import { Redirect } from 'react-router'; 
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { CreatePresentation } from './components/CreatePresentation';
import { BoardReader } from './components/BoardReader';
import { StatisticsPresentationList } from './components/StatisticsPresentationList';
import { StatisticsBoardList } from './components/StatisticsBoardList';
import { StatisticsConnectionList } from './components/StatisticsConnectionList';
import { Login } from './components/Login';
import { Logout } from './components/Logout';
import { PresentationList } from './components/PresentationList';
import { EditPresentation } from './components/EditPresentation';
import { NavMenu } from './components/NavMenu';
import { BoardReaderFromUrl } from './components/BoardReaderFromUrl';

export const routes = <Layout>
    <Route path='/login' component={Login} />
    <Route path='/logout' component={Logout} />
    <Route exact path='/admin' component={PresentationList} />
    <Route exact path='/admin/presentations' component={PresentationList} />
    <Route path='/admin/presentations/create' component={CreatePresentation} />
    <Route path='/admin/presentations/edit/:id' component={EditPresentation} />
    <Route exact path='/admin/statistics' component={StatisticsPresentationList} />
    <Route exact path="/admin/statistics/:presentationId" component={StatisticsBoardList} />
    <Route exact path="/admin/statistics/:presentationId/:boardId" component={StatisticsConnectionList} />
    <Route exact path='/' component={Login} />
    <Route path='/' component={NavMenu} />
    <Route exact path='/p/:id' component={BoardReader} />
    <Route exact path='/k/:presentationId/:boardId' component={BoardReaderFromUrl} />
</Layout>;